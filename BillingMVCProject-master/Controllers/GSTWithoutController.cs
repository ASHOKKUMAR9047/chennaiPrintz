using BillingMVCProject.Models;
using BillingMVCProject.UIViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BillingMVCProject.Controllers
{
    public class GSTWithoutController : Controller
    {
        private readonly billingContext _context;

        public GSTWithoutController(billingContext context)
        {
            _context = context;
        }

        // GET: Billings
        public async Task<IActionResult> Index()
        {
            var customerdetails = await _context.CustomersDetails.ToListAsync();
            return View(customerdetails);
        }
        [HttpPost]
        public IActionResult Create(CustomersDetail customer)
        {
            if (ModelState.IsValid)
            {
                _context.CustomersDetails.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to the customer list page
            }
            return View(customer); // Return to the create view if the model state is invalid
        }
        public IActionResult Edit(int id)
        {
            var product = _context.CustomersDetails.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_EditModal", product);
        }

        // POST: Products/Edit

        [HttpPost]
        public IActionResult Edit(CustomersDetail customer) // Accepting the entire customer model
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = _context.CustomersDetails.Find(customer.CustomerId);
                if (existingCustomer != null)
                {
                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.EmailId = customer.EmailId;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.GstNumber = customer.GstNumber;

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(customer); // Return to the view with an error message if needed
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var customer = _context.CustomersDetails.Find(id);
            if (customer == null)
            {
                return NotFound(); // Return 404 if customer not found
            }

            _context.CustomersDetails.Remove(customer); // Remove the customer from the context
            _context.SaveChanges(); // Save changes to the database
            return RedirectToAction("Index"); // Redirect back to the Index action
        }
        public IActionResult ViewDetails(int id)
        {

            var result1 = from customer in _context.CustomersDetails
                         join invoice in _context.InvoiceDetails
                         on customer.CustomerId equals invoice.CustomerId into invoiceGroup
                         from invoice in invoiceGroup.DefaultIfEmpty()
                         where  (invoice.PaymentStatus == "UnPaid" || invoice.PaymentStatus == "PartiallyPaid") && customer.CustomerId == id
                         select new InvoiceCustomerDetailsViewModel
                         {
                             CustomerId = customer.CustomerId,
                             CustomerName = customer.CustomerName,
                             InvoiceId = invoice.InvoiceId,
                             InvoiceDate= invoice.Invoicedate.ToString(),
                             PaymentStatus = invoice.PaymentStatus,
                             InvoiceOutStandingAmount = invoice.InvoiceOutStandingAmount
                         };

            var totalOutstanding = _context.InvoiceDetails
                                .Where(invoice => (invoice.PaymentStatus == "UnPaid" || invoice.PaymentStatus == "PartiallyPaid")
                                                  && invoice.CustomerId == id)
                                .Sum(invoice => invoice.InvoiceOutStandingAmount);

            var result = new InvoiceDetailsWithSum
            {
                InvoiceDetails = result1.ToList(),
                TotalOutstandingAmount = totalOutstanding
            };

            return View(result);
        }
        [HttpGet]
        public IActionResult WithOutGSTInvoice()
        {
            var customers = _context.CustomersDetails.Select(c => new CustomerViewModel
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                Address = c.Address,
                Email = c.EmailId,
                Phone = c.PhoneNumber
            }).ToList();

            var now = DateTime.Now;

            var invoiceid = _context.Invoicesgenerates.FirstOrDefault();
            string invoiceNumber =invoiceid.Invoiceid;
            //if (invoiceid != null)
            //{
                
            //    invoiceNumber = invoiceid.Invoiceid;
            //    string prefix = invoiceNumber.Substring(0, 2); // "EC"
            //    string numberPart = invoiceNumber.Substring(2); // "00001"

                
            //    int number = int.Parse(numberPart);
            //    number++; 

                
            //    string newNumberPart = number.ToString().PadLeft(numberPart.Length, '0');

               
            //    NewInvoiceId = prefix + newNumberPart;


            //    //invoiceid.Invoiceid = newInvoiceId;

            //    string updateSql = "UPDATE billing.invoicesgenerate SET InvoiceId = @NewInvoiceId WHERE InvoiceId = @OldInvoiceId";

            //    _context.Database.ExecuteSqlRaw(updateSql,
            //        new MySqlParameter("@NewInvoiceId", NewInvoiceId),  // New InvoiceId to set
            //        new MySqlParameter("@OldInvoiceId", invoiceNumber)   // The current InvoiceId you're updating from
            //    );
            //}
           
            //string invoiceNumber = $"CP_{now:yyyyMMddHHmmssfff}";

            var viewModel = new InvoiceViewModel
            {
                Customers = customers, // Pass customers to the view
                InvoiceId = invoiceNumber

            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult SaveInvoice(InvoiceViewModel model) // Create a view model for invoice
        {
            try
            {
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = model.InvoiceId,
                    CustomerId = model.CustomerId,
                    Notes = model.Notes,
                    Invoicedate = DateOnly.FromDateTime(model.InvoiceDate),
                    DeliveryCharge = model.DeliveryCharge,
                    GstAmount = model.GstAmount,
                    GrandTotal = model.GrandTotal,
                    CreatedAt = DateTime.Now,
                    PaymentStatus="UnPaid",
                    InvoiceOutStandingAmount=model.GrandTotal,
                    
                };

                _context.InvoiceDetails.Add(invoiceDetail);
                _context.SaveChanges();

                var invoiceid = _context.Invoicesgenerates.FirstOrDefault();
                string invoiceNumber = ""; string NewInvoiceId = "";
                if (invoiceid != null)
                {

                    invoiceNumber = invoiceid.Invoiceid;
                    string prefix = invoiceNumber.Substring(0, 2); // "EC"
                    string numberPart = invoiceNumber.Substring(2); // "00001"


                    int number = int.Parse(numberPart);
                    number++;


                    string newNumberPart = number.ToString().PadLeft(numberPart.Length, '0');


                    NewInvoiceId = prefix + newNumberPart;


                    //invoiceid.Invoiceid = newInvoiceId;

                    string updateSql = "UPDATE billing.invoicesgenerate SET InvoiceId = @NewInvoiceId WHERE InvoiceId = @OldInvoiceId";

                    _context.Database.ExecuteSqlRaw(updateSql,
                        new MySqlParameter("@NewInvoiceId", NewInvoiceId),  // New InvoiceId to set
                        new MySqlParameter("@OldInvoiceId", invoiceNumber)   // The current InvoiceId you're updating from
                    );
                }

                if (model.Items.Count > 0)
                {
                    foreach (var item in model.Items)
                    {
                        if (item.Gsm != null || item.Description !=null)
                        {
                            var invoiceItem = new Invoiceitemdetail // Adjust as per actual item class name
                            {
                                InvoiceId = invoiceDetail.InvoiceId, // Link item to the main invoice
                                Description = item.Description,
                                Size = item.Size,
                                Gsm = item.Gsm,
                                Quantity = item.Quantity,
                                Side = item.Side,
                                UnitPrice = item.UnitPrice,
                                TotalValues = item.Total
                            };

                            _context.Invoiceitemdetails.Add(invoiceItem); // Add each item to context
                            _context.SaveChanges();
                            

                            TempData["SuccessMessage"] = "Invoice created successfully.";

                        }
                    }

                        // Save all changes to the database
                        //_context.SaveChanges();
                    
                }

            }
            catch (Exception ex)
            {
                // Set failure message in TempData
                TempData["ErrorMessage"] = "An error occurred while saving the invoice: " + ex.Message;
            }

            return RedirectToAction("WithOutGSTInvoiceList"); // Redirect to an appropriate page
        }
        [HttpPost]
        public ActionResult RecordPayment(string InvoiceId, DateTime InvoiceDate, string PaymentMode, decimal PaymentAmount)
        {
            try
            {
                
                var OutStandingAmount = _context.InvoiceDetails
               .Where(item => item.InvoiceId == InvoiceId && item.Invoicedate == DateOnly.FromDateTime(InvoiceDate))
               .FirstOrDefault();
                

                    var OutstandingResult = OutStandingAmount.InvoiceOutStandingAmount - PaymentAmount;

                    if (OutstandingResult > 0)
                    {
                        string updateSql = "UPDATE invoice_details " +
                       "SET InvoiceOutStandingAmount = @OutstandingAmount,PaymentMode=@PaymentMode, PaymentStatus = @PaymentStatus " +
                       "WHERE InvoiceDate = @InvoiceDate AND InvoiceId = @InvoiceId;";

                        _context.Database.ExecuteSqlRaw(updateSql,
                            new MySqlParameter("@OutstandingAmount", OutstandingResult),
                            new MySqlParameter("@PaymentMode", PaymentMode),
                            new MySqlParameter("@PaymentStatus", "PartiallyPaid"),
                            new MySqlParameter("@InvoiceDate", OutStandingAmount.Invoicedate),
                            new MySqlParameter("@InvoiceId", OutStandingAmount.InvoiceId)
                        );
                    }
                    else
                    {
                        string updateSql = "UPDATE invoice_details " +
                       "SET InvoiceOutStandingAmount = @OutstandingAmount,PaymentMode=@PaymentMode, PaymentStatus = @PaymentStatus " +
                       "WHERE InvoiceDate = @InvoiceDate AND InvoiceId = @InvoiceId;";

                        _context.Database.ExecuteSqlRaw(updateSql,
                            new MySqlParameter("@OutstandingAmount", OutstandingResult),
                            new MySqlParameter("@PaymentMode", PaymentMode),
                            new MySqlParameter("@PaymentStatus", "Paid"),
                            new MySqlParameter("@InvoiceDate", OutStandingAmount.Invoicedate),
                            new MySqlParameter("@InvoiceId", OutStandingAmount.InvoiceId)
                        );

                    }
                

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle error
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteInvoice(string idvalues, string invoiceDate)
        {
            // Try parsing the invoice date if needed (ensure it's valid)
            
            if (!DateOnly.TryParse(invoiceDate, out DateOnly parsedInvoiceDate))
            {
                return Json(new { success = false, message = "Invalid Invoice Date." });
            }
            var invoiceItems = _context.Invoiceitemdetails
                                .Where(item => item.InvoiceId == idvalues)
                                .ToList();

            if (invoiceItems.Count > 0)
            {
                try
                {
                    // Remove the related invoice items
                    _context.Invoiceitemdetails.RemoveRange(invoiceItems);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error removing invoice items: " + ex.Message });
                }
            }
            // Now compare the InvoiceId and parsedInvoiceDate
            var invoice = _context.InvoiceDetails
                                  .Where(invoi => invoi.InvoiceId == idvalues && invoi.Invoicedate == parsedInvoiceDate)
                                  .FirstOrDefault();

            // If the invoice exists
            if (invoice != null)
            {
                try
                {
                    // Remove the invoice from the database
                    _context.InvoiceDetails.Remove(invoice);

                    // Save the changes to the database
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Invoice deleted successfully." });
                }
                catch (Exception ex)
                {
                    // If any exception occurs, return an error message
                    return Json(new { success = false, message = "An error occurred while deleting the invoice: " + ex.Message });
                }
            }
            else
            {
                // If no invoice is found
                return Json(new { success = false, message = "Invoice not found." });
            }
        }


        [HttpGet]
        public IActionResult GetCustomerDetails(int customerId)
        {
            // Fetch the customer from the database based on the ID
            var customer = _context.CustomersDetails
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Address = c.Address,
                    Email = c.EmailId,
                    Phone = c.PhoneNumber
                })
                .FirstOrDefault();

            // If the customer is not found, return a 404 error
            if (customer == null)
            {
                return NotFound();
            }

            // Return the customer data as JSON
            return Json(customer);
        }
        [HttpGet]
        public IActionResult WithOutGSTInvoiceList()
        {
            
            var result = from invoice in _context.InvoiceDetails
                         join customer in _context.CustomersDetails
                         on invoice.CustomerId equals customer.CustomerId
                         orderby invoice.CreatedAt descending
                         select new InvoiceCustomerDetail
                         {
                             InvoiceId = invoice.InvoiceId,
                             CustomerId = invoice.CustomerId,
                             Notes = invoice.Notes,
                             Invoicedate = invoice.Invoicedate,
                             DeliveryCharge = invoice.DeliveryCharge,
                             GstAmount = invoice.GstAmount,
                             GrandTotal = invoice.GrandTotal,
                             CustomerName = customer.CustomerName,
                             PaymentStatus = invoice.PaymentStatus
                         };

            var resultList = result.ToList();

            return View(resultList);
        }
        public IActionResult FilterInvoices(DateTime? invoiceDate, string search)
        {
            // Retrieve all invoices from the database
            var result = from invoice in _context.InvoiceDetails
                         join customer in _context.CustomersDetails
                         on invoice.CustomerId equals customer.CustomerId
                         select new InvoiceCustomerDetail
                         {
                             InvoiceId = invoice.InvoiceId,
                             CustomerId = invoice.CustomerId,
                             Notes = invoice.Notes,
                             Invoicedate = invoice.Invoicedate,
                             DeliveryCharge = invoice.DeliveryCharge,
                             GstAmount = invoice.GstAmount,
                             GrandTotal = invoice.GrandTotal,
                             CustomerName = customer.CustomerName
                         };

            // Filter by date if invoiceDate is provided
            if (invoiceDate.HasValue)
            {
                var selectedDate = DateOnly.FromDateTime(invoiceDate.Value);
                result = result.Where(i => i.Invoicedate == selectedDate);
            }

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(i => i.CustomerName.Contains(search) || i.InvoiceId.Contains(search));
            }

            var invoiceCustomerDetails = result.ToList();

            return View("WithOutGSTInvoiceList", invoiceCustomerDetails);
        }



        [HttpPost]
        public IActionResult ViewInvoiceDetails(string id, string flag)
        {


            //var invoiceDetails = (from inv in _context.InvoiceDetails
            //                      join item in _context.Invoiceitemdetails on inv.InvoiceId equals item.InvoiceId
            //                      join cust in _context.CustomersDetails on inv.CustomerId equals cust.CustomerId
            //                      where inv.InvoiceId == id
            //                      group new { inv, item, cust } by new
            //                      {
            //                          inv.InvoiceId,
            //                          inv.Invoicedate,
            //                          inv.DeliveryCharge,
            //                          inv.GstAmount
            //                      } into g
            //                      select new InvoiceDetailViewModel
            //                      {
            //                          InvoiceId = g.Key.InvoiceId,
            //                          Invoicedate = g.Key.Invoicedate,
            //                          DeliveryCharge = g.Key.DeliveryCharge,
            //                          GstAmount = g.Key.GstAmount,
            //                          flag = flag,
            //                          Items = g.Select(x => new InvoiceItemDetail
            //                          {
            //                              Gsm = x.item.Gsm,
            //                              Description = x.item.Description,
            //                              Quantity = x.item.Quantity,
            //                              Size = x.item.Size,
            //                              Side = x.item.Side,
            //                              UnitPrice = x.item.UnitPrice,
            //                              Total = x.item.Quantity * x.item.UnitPrice
            //                          }).ToList(),
            //                          Customers = g.Select(x => new CustomerDetailViewModel
            //                          {
            //                              CustomerId = x.cust.CustomerId,
            //                              CustomerName = x.cust.CustomerName,
            //                              PhoneNumber = x.cust.PhoneNumber,
            //                              EmailId = x.cust.EmailId,
            //                              Address = x.cust.Address,
            //                              GstNumber = x.cust.GstNumber
            //                          }).Distinct().ToList()
            //                      }).FirstOrDefault();
            //var invoiceDetails = (from inv in _context.InvoiceDetails
            //                      join item in _context.Invoiceitemdetails on inv.InvoiceId equals item.InvoiceId
            //                      join cust in _context.CustomersDetails on inv.CustomerId equals cust.CustomerId
            //                      where inv.InvoiceId == id
            //                      group new { inv, item, cust } by new
            //                      {
            //                          inv.InvoiceId,
            //                          inv.Invoicedate,
            //                          inv.DeliveryCharge,
            //                          inv.GstAmount,
            //                          inv.GrandTotal,
            //                          inv.InvoiceOutStandingAmount,
            //                          cust.CustomerName,
            //                          cust.PhoneNumber,
            //                          cust.EmailId,
            //                          cust.Address,
            //                          cust.GstNumber,
            //                          cust.CustomerId
            //                      } into g
            //                      select new InvoiceDetailViewModel
            //                      {
            //                          InvoiceId = g.Key.InvoiceId,
            //                          Invoicedate = g.Key.Invoicedate,
            //                          DeliveryCharge = g.Key.DeliveryCharge,
            //                          GstAmount = g.Key.GstAmount,
            //                          InvoiceOutStanding = g.Key.InvoiceOutStandingAmount,
            //                          Items = g.Select(x => new InvoiceItemDetail
            //                          {
            //                              Gsm = x.item.Gsm,
            //                              ItemId = x.item.ItemId,
            //                              Description = x.item.Description,
            //                              Quantity = x.item.Quantity,
            //                              Size = x.item.Size,
            //                              Side = x.item.Side,
            //                              Total = x.item.TotalValues.HasValue ? (decimal)x.item.TotalValues : 0m
            //                          }).ToList(),
            //                          Customer_id = g.Key.CustomerId, // Capture selected Customer ID
            //                          CustomerName = g.Key.CustomerName,
            //                          PhoneNumber = g.Key.PhoneNumber,
            //                          EmailId = g.Key.EmailId,
            //                          Address = g.Key.Address,
            //                          GstNumber = g.Key.GstNumber,
            //                          flag = flag,
            //                          GrandTotal = g.Key.GrandTotal,
            //                          Customers = _context.CustomersDetails.Select(c => new CustomerDetailViewModel
            //                          {
            //                              CustomerId = c.CustomerId,
            //                              CustomerName = c.CustomerName,
            //                              PhoneNumber = c.PhoneNumber,
            //                              EmailId = c.EmailId,
            //                              Address = c.Address,
            //                              GstNumber = c.GstNumber
            //                          }).ToList()
            //                      }).FirstOrDefault();

            //var roundedGrandTotal = Convert.ToInt32(Math.Round(invoiceDetails.GrandTotal)); // Round to the nearest integer
            //var GrandTotalWords = ConvertToWords(roundedGrandTotal);

            var invoiceDetails = (from inv in _context.InvoiceDetails
                                  join item in _context.Invoiceitemdetails on inv.InvoiceId equals item.InvoiceId
                                  join cust in _context.CustomersDetails on inv.CustomerId equals cust.CustomerId
                                  where inv.InvoiceId == id
                                  group new { inv, item, cust } by new
                                  {
                                      inv.InvoiceId,
                                      inv.Invoicedate,
                                      inv.DeliveryCharge,
                                      inv.GstAmount,
                                      inv.GrandTotal,
                                      inv.InvoiceOutStandingAmount,
                                      cust.CustomerName,
                                      cust.PhoneNumber,
                                      cust.EmailId,
                                      cust.Address,
                                      cust.GstNumber,
                                      cust.CustomerId
                                  } into g
                                  select new InvoiceDetailViewModel
                                  {
                                      InvoiceId = g.Key.InvoiceId,
                                      Invoicedate = g.Key.Invoicedate,
                                      DeliveryCharge = g.Key.DeliveryCharge,
                                      GstAmount = g.Key.GstAmount,
                                      InvoiceOutStanding = g.Key.InvoiceOutStandingAmount,
                                      Items = g.Select(x => new InvoiceItemDetail
                                      {
                                          Gsm = x.item.Gsm,
                                          ItemId = x.item.ItemId,
                                          Description = x.item.Description,
                                          Quantity = x.item.Quantity,
                                          Size = x.item.Size,
                                          Side = x.item.Side,
                                          Total = x.item.TotalValues.HasValue ? (decimal)x.item.TotalValues : 0m
                                      }).ToList(),
                                      Customer_id = g.Key.CustomerId,
                                      CustomerName = g.Key.CustomerName,
                                      PhoneNumber = g.Key.PhoneNumber,
                                      EmailId = g.Key.EmailId,
                                      Address = g.Key.Address,
                                      GstNumber = g.Key.GstNumber,
                                      flag = flag,
                                      GrandTotal = g.Key.GrandTotal,
                                      Customers = _context.CustomersDetails.Select(c => new CustomerDetailViewModel
                                      {
                                          CustomerId = c.CustomerId,
                                          CustomerName = c.CustomerName,
                                          PhoneNumber = c.PhoneNumber,
                                          EmailId = c.EmailId,
                                          Address = c.Address,
                                          GstNumber = c.GstNumber
                                      }).ToList()
                                  }).FirstOrDefault();

            // After retrieving the data, call ConvertToWords on the GrandTotal
            if (invoiceDetails != null)
            {
                var roundedGrandTotal = Convert.ToInt32(Math.Round(invoiceDetails.GrandTotal)); // Round to the nearest integer
                invoiceDetails.GrandTotalWords = ConvertToWords(roundedGrandTotal); // Convert to words
            }

            return PartialView("_WithOutGSTInvoiceDetailsPartial", invoiceDetails);

        }
       public string ConvertToWords(int n)
       {
            if (n == 0)
                return "Zero";

            // Words for numbers 0 to 19
            string[] units = {
            "",        "One",       "Two",      "Three",
            "Four",    "Five",      "Six",      "Seven",
            "Eight",   "Nine",      "Ten",      "Eleven",
            "Twelve",  "Thirteen",  "Fourteen", "Fifteen",
            "Sixteen", "Seventeen", "Eighteen", "Nineteen"
           };

            // Words for numbers multiple of 10        
            string[] tens = {
            "",     "",     "Twenty",  "Thirty", "Forty",
            "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
           };

            string[] multiplier =
                        {"", "Thousand", "Million", "Billion"};

            string res = "";
            int group = 0;

            // Process number in group of 1000s
            while (n > 0)
            {
                if (n % 1000 != 0)
                {

                    int value = n % 1000;
                    string temp = "";

                    // Handle 3 digit number
                    if (value >= 100)
                    {
                        temp = units[value / 100] + " Hundred ";
                        value %= 100;
                    }

                    // Handle 2 digit number
                    if (value >= 20)
                    {
                        temp += tens[value / 10] + " ";
                        value %= 10;
                    }

                    // Handle unit number
                    if (value > 0)
                    {
                        temp += units[value] + " ";
                    }

                    // Add the multiplier according to the group
                    temp += multiplier[group] + " ";

                    // Add the this group result to overall result
                    res = temp + res;
                }
                n /= 1000;
                group++;
            }

            // Remove trailing space
            return res.Trim();
        }

        [HttpPost]
        public IActionResult UpdateInvoice(InvoiceViewModel model)
        {
            //try
            //{


            //    if (model != null)
            //    {
            //        var CustomerIdCheck = _context.InvoiceDetails.Where(x => x.InvoiceId == model.InvoiceId).ToList();
            //        if (model.CustomerId != CustomerIdCheck[0].CustomerId)
            //        {
            //            string updateSql = "UPDATE invoice_details " +
            //             "SET CustomerId = {0}, DeliveryCharge = {1}, GstAmount = {2}, GrandTotal = {3} " +  // Removed extra comma here
            //             "WHERE InvoiceDate = {4} AND InvoiceId = {5};";

            //            _context.Database.ExecuteSqlRaw(updateSql, model.CustomerId, model.DeliveryCharge, model.GstAmount, model.GrandTotal, model.InvoiceDate, model.InvoiceId);

            //        }
            //        else if (CustomerIdCheck[0].GrandTotal != model.GrandTotal)
            //        {
            //            string updateSql = "UPDATE invoice_details " +
            //             "SET CustomerId = {0}, DeliveryCharge = {1}, GstAmount = {2}, GrandTotal = {3} " +  // Removed extra comma here
            //             "WHERE InvoiceDate = {4} AND InvoiceId = {5};";

            //            _context.Database.ExecuteSqlRaw(updateSql, model.CustomerId, model.DeliveryCharge, model.GstAmount, model.GrandTotal, model.InvoiceDate, model.InvoiceId);
            //        }

            //        if (model.Items.Count > 0)
            //        {
            //            foreach (var item in model.Items)
            //            {
            //                var ItemIdcheck = _context.Invoiceitemdetails
            //                       .Where(Itemsvalues => Itemsvalues.ItemId == item.Itemid)
            //                       .ToList();

            //                if (ItemIdcheck.Count > 0)
            //                {
            //                    // Update the record if it already exists
            //                    string updateLineItems = "UPDATE invoiceitemdetails " +
            //                 "SET Description = @Description, Size = @Size, Gsm = @Gsm, " +
            //                 "Quantity = @Quantity, Side = @Side, UnitPrice = @UnitPrice, TotalValues = @Total " +  // Space added here
            //                 "WHERE InvoiceId = @InvoiceId AND ItemId = @ItemId;";

            //                    _context.Database.ExecuteSqlRaw(updateLineItems,
            //                        new MySqlParameter("@Description", item.Description),
            //                        new MySqlParameter("@Size", item.Size),
            //                        new MySqlParameter("@Gsm", item.Gsm),
            //                        new MySqlParameter("@Quantity", item.Quantity),
            //                        new MySqlParameter("@Side", item.Side),
            //                        new MySqlParameter("@UnitPrice", item.UnitPrice),
            //                        new MySqlParameter("@Total", item.Total),  // Make sure this parameter name matches the query
            //                        new MySqlParameter("@InvoiceId", model.InvoiceId),
            //                        new MySqlParameter("@ItemId", item.Itemid));
            //                }
            //                else
            //                {

            //                    string insertLineItems = "INSERT INTO invoiceitemdetails " +
            //                                             "(InvoiceId, ItemId, Description, Size, Gsm, Quantity, Side, UnitPrice, TotalValues) " +
            //                                             "VALUES (@InvoiceId, @ItemId, @Description, @Size, @Gsm, @Quantity, @Side, @UnitPrice, @TotalValues);";

            //                    // Execute the insert command using parameters
            //                    _context.Database.ExecuteSqlRaw(insertLineItems,
            //                        new MySqlParameter("@InvoiceId", model.InvoiceId),
            //                        new MySqlParameter("@ItemId", item.Itemid),
            //                        new MySqlParameter("@Description", item.Description),
            //                        new MySqlParameter("@Size", item.Size),
            //                        new MySqlParameter("@Gsm", item.Gsm),
            //                        new MySqlParameter("@Quantity", item.Quantity),
            //                        new MySqlParameter("@Side", item.Side),
            //                        new MySqlParameter("@UnitPrice", item.UnitPrice),
            //                        new MySqlParameter("@TotalValues", item.Total));
            //                }
            //            }
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception (optional)
            //    TempData["FailureMessage"] = "An error occurred while updating the invoice. Please try again.";
            //}
            try
            {
                if (model != null)
                {
                    bool isUpdated = false; // To track if any update occurs
                    string formattedDate = model.InvoiceDate.ToString("yyyy-MM-dd");
                    var CustomerIdCheck = _context.InvoiceDetails.Where(x => x.InvoiceId == model.InvoiceId).ToList();
                    string InvoiceDateview = CustomerIdCheck[0].Invoicedate.ToString("yyyy-MM-dd");
                    //if (model.CustomerId != CustomerIdCheck[0].CustomerId)
                    //{
                    //    string updateSql = "UPDATE invoice_details " +
                    //                       "SET CustomerId = {0}, DeliveryCharge = {1}, GstAmount = {2}, GrandTotal = {3},Invoicedate{4} " +
                    //                       "WHERE InvoiceDate = {5} AND InvoiceId = {6};";

                    //    _context.Database.ExecuteSqlRaw(updateSql, model.CustomerId, model.DeliveryCharge, model.GstAmount, model.GrandTotal, formattedDate, InvoiceDateview, model.InvoiceId);
                    //    isUpdated = true;
                    //}
                    //else if (CustomerIdCheck[0].GrandTotal != model.GrandTotal)
                    //{
                    //    string updateSql = "UPDATE invoice_details " +
                    //                       "SET CustomerId = {0}, DeliveryCharge = {1}, GstAmount = {2}, GrandTotal = {3},Invoicedate{4} " +
                    //                       "WHERE InvoiceDate = {5} AND InvoiceId = {6};";

                    //    _context.Database.ExecuteSqlRaw(updateSql, model.CustomerId, model.DeliveryCharge, model.GstAmount, model.GrandTotal, formattedDate, InvoiceDateview, model.InvoiceId);
                    //    isUpdated = true;
                    //}
                    //else
                    //{
                    //    string updateSql = "UPDATE invoice_details " +
                    //                       "SET CustomerId = {0}, DeliveryCharge = {1}, GstAmount = {2}, GrandTotal = {3},Invoicedate{4} " +
                    //                       "WHERE InvoiceDate = {5} AND InvoiceId = {6};";

                    //    _context.Database.ExecuteSqlRaw(updateSql, model.CustomerId, model.DeliveryCharge, model.GstAmount, model.GrandTotal, formattedDate, InvoiceDateview, model.InvoiceId);
                    //    isUpdated = true;
                    //}

                    if (model.CustomerId != CustomerIdCheck[0].CustomerId)
                    {
                        string updateSql = "UPDATE invoice_details " +
                             "SET CustomerId = @CustomerId, DeliveryCharge = @DeliveryCharge, GstAmount = @GstAmount, " +
                             "GrandTotal = @GrandTotal,InvoiceOutStandingAmount=@InvoiceOutStandingAmount, Invoicedate = @Invoicedate " +
                             "WHERE  InvoiceId = @InvoiceId;";

                        var result = _context.Database.ExecuteSqlRaw(updateSql,
                                                        new MySqlParameter("@CustomerId", model.CustomerId),
                                                        new MySqlParameter("@DeliveryCharge", model.DeliveryCharge),
                                                        new MySqlParameter("@GstAmount", model.GstAmount),
                                                        new MySqlParameter("@GrandTotal", model.GrandTotal),
                                                        new MySqlParameter("@InvoiceOutStandingAmount", model.GrandTotal),
                                                        new MySqlParameter("@Invoicedate", formattedDate),
                                                        new MySqlParameter("@InvoiceId", model.InvoiceId));

                        if (result > 0)
                        {
                            isUpdated = true; // Rows were updated
                        }
                        else
                        {
                            isUpdated = false; // No rows were updated
                                               // Log additional info for debugging, e.g., if the InvoiceId exists with the provided InvoiceDate
                        }
                    }
                    else if (CustomerIdCheck[0].GrandTotal != model.GrandTotal)
                    {
                        string updateSql = "UPDATE invoice_details " +
                             "SET CustomerId = @CustomerId, DeliveryCharge = @DeliveryCharge, GstAmount = @GstAmount, " +
                             "GrandTotal = @GrandTotal,InvoiceOutStandingAmount=@InvoiceOutStandingAmount, Invoicedate = @Invoicedate " +
                             "WHERE  InvoiceId = @InvoiceId;";

                        var result = _context.Database.ExecuteSqlRaw(updateSql,
                                                        new MySqlParameter("@CustomerId", model.CustomerId),
                                                        new MySqlParameter("@DeliveryCharge", model.DeliveryCharge),
                                                        new MySqlParameter("@GstAmount", model.GstAmount),
                                                        new MySqlParameter("@GrandTotal", model.GrandTotal),
                                                        new MySqlParameter("@InvoiceOutStandingAmount", model.GrandTotal),
                                                        new MySqlParameter("@Invoicedate", formattedDate),
                                                        new MySqlParameter("@InvoiceId", model.InvoiceId));

                        if (result > 0)
                        {
                            isUpdated = true; // Rows were updated
                        }
                        else
                        {
                            isUpdated = false; // No rows were updated
                                               // Log additional info for debugging, e.g., if the InvoiceId exists with the provided InvoiceDate
                        }
                        //string updateSql = "UPDATE invoice_details " +
                        //                   "SET CustomerId = @CustomerId, DeliveryCharge = @DeliveryCharge, GstAmount = @GstAmount, " +
                        //                   "GrandTotal = @GrandTotal, Invoicedate = @Invoicedate " +
                        //                   "WHERE Invoicedate = @InvoiceDate AND InvoiceId = @InvoiceId;";

                        //_context.Database.ExecuteSqlRaw(updateSql,
                        //                                new MySqlParameter("@CustomerId", model.CustomerId),
                        //                                new MySqlParameter("@DeliveryCharge", model.DeliveryCharge),
                        //                                new MySqlParameter("@GstAmount", model.GstAmount),
                        //                                new MySqlParameter("@GrandTotal", model.GrandTotal),
                        //                                new MySqlParameter("@Invoicedate", formattedDate),
                        //                                new MySqlParameter("@InvoiceDate", InvoiceDateview),
                        //                                new MySqlParameter("@InvoiceId", model.InvoiceId));
                        //isUpdated = true;
                    }
                    else
                    {
                        //string updateSql = "UPDATE invoice_details " +
                        //                   "SET CustomerId = @CustomerId, DeliveryCharge = @DeliveryCharge, GstAmount = @GstAmount, " +
                        //                   "GrandTotal = @GrandTotal, Invoicedate = @Invoicedate " +
                        //                   "WHERE Invoicedate = @InvoiceDate AND InvoiceId = @InvoiceId;";

                        //_context.Database.ExecuteSqlRaw(updateSql,
                        //                                new MySqlParameter("@CustomerId", model.CustomerId),
                        //                                new MySqlParameter("@DeliveryCharge", model.DeliveryCharge),
                        //                                new MySqlParameter("@GstAmount", model.GstAmount),
                        //                                new MySqlParameter("@GrandTotal", model.GrandTotal),
                        //                                new MySqlParameter("@Invoicedate", formattedDate),
                        //                                new MySqlParameter("@InvoiceDate", InvoiceDateview),
                        //                                new MySqlParameter("@InvoiceId", model.InvoiceId));
                        //isUpdated = true;
                        string updateSql = "UPDATE invoice_details " +
                   "SET CustomerId = @CustomerId, DeliveryCharge = @DeliveryCharge, GstAmount = @GstAmount, " +
                   "GrandTotal = @GrandTotal,InvoiceOutStandingAmount=@InvoiceOutStandingAmount, Invoicedate = @Invoicedate " +
                   "WHERE  InvoiceId = @InvoiceId;";

                        var result = _context.Database.ExecuteSqlRaw(updateSql,
                                                        new MySqlParameter("@CustomerId", model.CustomerId),
                                                        new MySqlParameter("@DeliveryCharge", model.DeliveryCharge),
                                                        new MySqlParameter("@GstAmount", model.GstAmount),
                                                        new MySqlParameter("@GrandTotal", model.GrandTotal),
                                                        new MySqlParameter("@InvoiceOutStandingAmount", model.GrandTotal),
                                                        new MySqlParameter("@Invoicedate", formattedDate),
                                                        new MySqlParameter("@InvoiceId", model.InvoiceId));

                        if (result > 0)
                        {
                            isUpdated = true; // Rows were updated
                        }
                        else
                        {
                            isUpdated = false; // No rows were updated
                                               // Log additional info for debugging, e.g., if the InvoiceId exists with the provided InvoiceDate
                        }
                    }

                    if (model.Items.Count > 0)
                    {
                        var existingItemIds = _context.Invoiceitemdetails
                       .Where(lineItem => lineItem.InvoiceId == model.InvoiceId)
                       .Select(lineItem => lineItem.ItemId)
                       .ToList();
                        
                        var pushedItemIds = model.Items.Select(item => item.Itemid).ToList();
                        
                        var extraItemIds = existingItemIds.Except(pushedItemIds).ToList();
                       
                        if (extraItemIds.Any())
                        {
                            var itemsToDelete = _context.Invoiceitemdetails
                                .Where(item => extraItemIds.Contains(item.ItemId) && item.InvoiceId == model.InvoiceId)
                                .ToList();

                            _context.Invoiceitemdetails.RemoveRange(itemsToDelete);
                            _context.SaveChanges();
                            isUpdated = true;
                        }
                        foreach (var item in model.Items)
                        {
                            var ItemIdcheck = _context.Invoiceitemdetails
                                   .Where(Itemsvalues => Itemsvalues.ItemId == item.Itemid)
                                   .ToList();

                            var InvoiceLineItemCheck = _context.Invoiceitemdetails.Where(lineItem => lineItem.InvoiceId == model.InvoiceId).ToList();

                            if (ItemIdcheck.Count > 0)
                            {
                                // Update existing line item
                                string updateLineItems = "UPDATE invoiceitemdetails " +
                                                         "SET Description = @Description, Size = @Size, Gsm = @Gsm, " +
                                                         "Quantity = @Quantity, Side = @Side, UnitPrice = @UnitPrice, TotalValues = @Total " +
                                                         "WHERE InvoiceId = @InvoiceId AND ItemId = @ItemId;";

                                _context.Database.ExecuteSqlRaw(updateLineItems,
                                    new MySqlParameter("@Description", item.Description),
                                    new MySqlParameter("@Size", item.Size),
                                    new MySqlParameter("@Gsm", item.Gsm),
                                    new MySqlParameter("@Quantity", item.Quantity),
                                    new MySqlParameter("@Side", item.Side),
                                    new MySqlParameter("@UnitPrice", item.UnitPrice),
                                    new MySqlParameter("@Total", item.Total),
                                    new MySqlParameter("@InvoiceId", model.InvoiceId),
                                    new MySqlParameter("@ItemId", item.Itemid));
                                isUpdated = true;
                            }
                            else
                            {
                                // Insert new line item
                                string insertLineItems = "INSERT INTO invoiceitemdetails " +
                                                         "(InvoiceId, ItemId, Description, Size, Gsm, Quantity, Side, UnitPrice, TotalValues) " +
                                                         "VALUES (@InvoiceId, @ItemId, @Description, @Size, @Gsm, @Quantity, @Side, @UnitPrice, @TotalValues);";

                                _context.Database.ExecuteSqlRaw(insertLineItems,
                                    new MySqlParameter("@InvoiceId", model.InvoiceId),
                                    new MySqlParameter("@ItemId", item.Itemid),
                                    new MySqlParameter("@Description", item.Description),
                                    new MySqlParameter("@Size", item.Size),
                                    new MySqlParameter("@Gsm", item.Gsm),
                                    new MySqlParameter("@Quantity", item.Quantity),
                                    new MySqlParameter("@Side", item.Side),
                                    new MySqlParameter("@UnitPrice", item.UnitPrice),
                                    new MySqlParameter("@TotalValues", item.Total));
                                isUpdated = true;
                            }
                        }
                    }

                    // Set the success message if something was updated
                    if (isUpdated)
                    {
                        TempData["SuccessMessage"] = "Invoice updated successfully!";
                    }
                    else
                    {
                        TempData["FailureMessage"] = "No changes were made to the invoice.";
                    }
                }
                else
                {
                    TempData["FailureMessage"] = "Invalid invoice data.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                TempData["FailureMessage"] = "An error occurred while updating the invoice. Please try again.";
            }
            return RedirectToAction("WithOutGSTInvoiceList");
        }


    }
}
