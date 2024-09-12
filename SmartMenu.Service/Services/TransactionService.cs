using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;


        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public IEnumerable<Transaction> GetAll(int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.TransactionRepository.EnableQuery();
            var result = DataQuery(data, transactionId, deviceSubscriptionId, searchString, pageNumber, pageSize);
            return result;
        }

        public async Task<Transaction> AddTransaction(TransactionCreateDTO transactionCreateDTO)
        {
            var deviceSubscription = await _unitOfWork.DeviceSubscriptionRepository.FindObjectAsync(c => c.DeviceSubscriptionId == transactionCreateDTO.DeviceSubscriptionId && c.IsDeleted == false)
                ?? throw new Exception("Device subscription not found or deleted.");

            var device = await _unitOfWork.StoreDeviceRepository.FindObjectAsync(c => c.StoreDeviceId == deviceSubscription.StoreDeviceId && c.IsDeleted == false)
                ?? throw new Exception("StoreDevice not found or deleted");

            var store = await _unitOfWork.StoreRepository.FindObjectAsync(c => c.StoreId == device.StoreId && c.IsDeleted == false)
                ?? throw new Exception("Store not found or deleted");

            var storeStaff = await _unitOfWork.BrandStaffRepository.EnableQuery()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.StoreId == device.StoreId && !c.IsDeleted)
                ?? throw new Exception("User not found or deleted");

            var subscription = await _unitOfWork.SubscriptionRepository.FindObjectAsync(c => c.SubscriptionId == deviceSubscription.SubscriptionId && c.IsDeleted == false)
                ?? throw new Exception("Subscription not found or deleted");

            var data = _mapper.Map<Transaction>(transactionCreateDTO);

            _unitOfWork.TransactionRepository.Add(data);
            _unitOfWork.Save();

            string body = @$"
                <h1>SmartMenu Transaction Details</h1>
                <p>TransactionID: {data.TransactionId}</p>
                <p>Device Name: {device.StoreDeviceName}/p>
                <p>Subscription Name: {subscription.Name}</p>
                <p>Subscription Start Date: {deviceSubscription.SubscriptionStartDate}</p>
                <p>Subscription End Date: {deviceSubscription.SubscriptionEndDate}</p>
                <table border = ""1"">
                    <thead>
                        <tr>
                            <th>Amount</th>
                            <th>Pay type</th>
                            <th>Pay date</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>{data.Amount}</td>
                            <td>{data.PayType.ToString()}</td>
                            <td>{data.PayDate.ToString("g")}</td>
                            <td>{data.Amount}</td>
                        </tr>
                    </tbody>
                </table>

                <p>Thank you for your support!</p>
                <p>Thanks,<br>
                    SmartMenu Team</p>
            ";

            string htmlContent = $@"<!-- Header -->
                <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#e1e1e1"">
                  <tr>
                    <td height=""20""></td>
                  </tr>
                  <tr>
                    <td>
                      <table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"" style=""border-radius: 10px 10px 0 0;"">
                        <tr class=""hiddenMobile"">
                          <td height=""40""></td>
                        </tr>
                        <tr>
                          <td>
                            <table width=""480"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                              <tbody>
                                <tr>
                                  <td>
                                    <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""left"" class=""col"">
                                      <tbody>
                                        <tr class=""hiddenMobile"">
                                          <td height=""40""></td>
                                        </tr>
                                        <tr class=""visibleMobile"">
                                          <td height=""20""></td>
                                        </tr>
                                        <tr>
                                          <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: left;"">
                                            Hello, {storeStaff.User!.UserName}.
                                            <br> Thank you for shopping from our store and for your order.
                                          </td>
                                        </tr>
                                      </tbody>
                                    </table>
                                    <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""right"" class=""col"">
                                      <tbody>
                                        <tr class=""visibleMobile"">
                                          <td height=""20""></td>
                                        </tr>
                                        <tr>
                                          <td height=""5""></td>
                                        </tr>
                                        <tr>
                                          <td style=""font-size: 30px; color: #ff0000; letter-spacing: -1px; font-family: 'Open Sans', sans-serif; line-height: 1; vertical-align: top; text-align: right;"">
                                            Invoice
                                          </td>
                                        </tr>
                                        <tr>

                                        <tr class=""visibleMobile"">
                                          <td height=""20""></td>
                                        </tr>

                                        <tr>
                                          <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: right;"">
                                            <small>TRANSACTION</small> #{data.TransactionId}<br />
                                            <small>{data.PayDate.ToString("g")}</small>
                                          </td>
                                        </tr>
                                      </tbody>
                                    </table>
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
                <!-- /Header -->
                <!-- Order Details -->
                <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#e1e1e1"">
                  <tbody>
                    <tr>
                      <td>
                        <table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"">
                          <tbody>
                            <tr class=""hiddenMobile"">
                              <td height=""60""></td>
                            </tr>
                            <tr class=""visibleMobile"">
                              <td height=""40""></td>
                            </tr>
                            <tr>
                              <td>
                                <table width=""480"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                  <tbody>
                                    <tr>
                                      <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 10px 7px 0;"" width=""52%"" align=""left"">
                                        Subscription
                                      </th>
                                      <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 0 7px;"" align=""left"">
                                        <small>Day Duration</small>
                                      </th>
                                      <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #1e2b33; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 0 7px;"" align=""right"">
                                        Price
                                      </th>
                                    </tr>
                                    <tr>
                                      <td height=""1"" style=""background: #bebebe;"" colspan=""4""></td>
                                    </tr>
                                    <tr>
                                      <td height=""10"" colspan=""4""></td>
                                    </tr>
                                    <tr>
                                      <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #ff0000;  line-height: 18px;  vertical-align: top; padding:10px 0;"" class=""article"">
                                        {subscription.Name}
                                      </td>
                                      <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e;  line-height: 18px;  vertical-align: top; padding:10px 0;"">
                                        <small>
                                            {subscription.DayDuration} 
                                        </small>
                                       </td>
                                      <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #1e2b33;  line-height: 18px;  vertical-align: top; padding:10px 0;"" align=""right"">
                                        {subscription.Price}
                                        </td>
                                    </tr>
                                    <tr>
                                      <td height=""1"" colspan=""4"" style=""border-bottom:1px solid #e4e4e4""></td>
                                    </tr>
                                  
                                  </tbody>
                                </table>
                              </td>
                            </tr>
                            <tr>
                              <td height=""20""></td>
                            </tr>
                          </tbody>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <!-- /Order Details -->
                <!-- Total -->
                <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#e1e1e1"">
                  <tbody>
                    <tr>
                      <td>
                        <table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"">
                          <tbody>
                            <tr>
                              <td>
                                <!-- Table Total -->
                                <table width=""480"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                  <tbody>
                                    <tr>
                                      <td style=""font-size: 15px; font-family: 'Open Sans', sans-serif; color: #000; line-height: 22px; vertical-align: top; text-align:right; width: 75%;"">
                                        <strong>Total</strong>
                                      </td>
                                      <td style=""font-size: 15px; font-family: 'Open Sans', sans-serif; color: #000; line-height: 22px; vertical-align: top; text-align:right; width: 75%;"">
                                        <strong>{subscription.Price}</strong>
                                      </td>
                                    </tr>
                                  </tbody>
                                </table>
                                <!-- /Table Total -->
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
                

                <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#e1e1e1"">

                  <tr>
                    <td>
                      <table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"" style=""border-radius: 0 0 10px 10px;"">
                        <tr>
                          <td>
                            <table width=""480"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                              <tbody>
                                <tr>
                                  <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: left;"">
                                    Have a nice day.
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                          </td>
                        </tr>
                        <tr class=""spacer"">
                          <td height=""50""></td>
                        </tr>

                      </table>
                    </td>
                  </tr>
                  <tr>
                    <td height=""20""></td>
                  </tr>
                </table>
            ";

            _emailService.SendEmail(new MessageCreateDTO(new string[] { storeStaff.User!.Email }, "Your purchase transaction", htmlContent));

            return data;
        }

        public async Task<Transaction> Update(int transactionId, TransactionUpdateDTO transactionUpdateDTO)
        {
            var transaction = await _unitOfWork.TransactionRepository.FindObjectAsync(c => c.TransactionId == transactionId && c.IsDeleted == false)
                ?? throw new Exception("Transaction not found or deleted.");

            _mapper.Map(transactionUpdateDTO, transaction);

            _unitOfWork.TransactionRepository.Update(transaction);
            _unitOfWork.Save();

            return transaction;
        }

        public void Delete(int transactionId)
        {
            var data = _unitOfWork.TransactionRepository.Find(c => c.TransactionId == transactionId && c.IsDeleted == false).FirstOrDefault()
                            ?? throw new Exception("Transaction not found or deleted.");

            data.IsDeleted = true;

            _unitOfWork.TransactionRepository.Update(data);
            _unitOfWork.Save();
        }

        private static IEnumerable<Transaction> DataQuery(IQueryable<Transaction> data, int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => !c.IsDeleted);

            if (transactionId != null)
            {
                data = data
                    .Where(c => c.TransactionId == transactionId);
            }

            if (deviceSubscriptionId != null)
            {
                data = data
                    .Where(c => c.DeviceSubscriptionId == deviceSubscriptionId);
            }

            if (searchString != null)
            {
                if (Enum.TryParse(typeof(PayType), searchString, out object? result))
                {
                    if (result != null)
                    {
                        data = data
                        .Where(c => c.PayType.Equals(result));
                    }
                }
            }

            return PaginatedList<Transaction>.Create(data, pageNumber, pageSize);
        }
    }
}