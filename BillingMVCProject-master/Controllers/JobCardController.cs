using BillingMVCProject.Models;
using BillingMVCProject.UIViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BillingMVCProject.Controllers
{
    public class JobCardController : Controller
    {
        private readonly billingContext _context;

        public JobCardController(billingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var customers = _context.CustomersDetails.Select(c => new Customer
            {
                Id = c.CustomerId,
                Name = c.CustomerName
            }).ToList();

            var JobCardId=_context.Jobcardgenerates.FirstOrDefault(); ;

            var model = new IndexViewModel
            {
                Customers = customers,
                JobCardId = JobCardId.JobCardId
            };

            return View(model);

        }

        [HttpPost]
        public IActionResult SaveJobCard(JobCardViewModel model)
        {
            try
            {
                if (model != null)
                {
                    string formattedDate = model.IssueDatevalues.ToString("yyyy-MM-dd");
                    var JobCard = new Jobcarddetail
                    {
                        JobCardId = model.JobCardId,
                        IssueDate = DateOnly.FromDateTime(model.IssueDatevalues),
                        CustomerId = model.CustomerId
                    };
                    _context.Jobcarddetails.Add(JobCard);
                    _context.SaveChanges();

                    var Jobid = _context.Jobcardgenerates.FirstOrDefault();
                    string JobCardnumber = ""; string NewJobCardnumber = "";
                    if (Jobid != null)
                    {

                        JobCardnumber = Jobid.JobCardId;
                        string prefix = JobCardnumber.Substring(0, 2); // "EC"
                        string numberPart = JobCardnumber.Substring(2); // "00001"


                        int number = int.Parse(numberPart);
                        number++;


                        string newNumberPart = number.ToString().PadLeft(numberPart.Length, '0');


                        NewJobCardnumber = prefix + newNumberPart;


                        //invoiceid.Invoiceid = newInvoiceId;

                        string updateSql = "UPDATE billing.jobcardgenerate SET JobCardId = @NewInvoiceId WHERE JobCardId = @OldInvoiceId";

                        _context.Database.ExecuteSqlRaw(updateSql,
                            new MySqlParameter("@NewInvoiceId", NewJobCardnumber),  // New InvoiceId to set
                            new MySqlParameter("@OldInvoiceId", JobCardnumber)   // The current InvoiceId you're updating from
                        );
                    }

                    if (model.Items.Count > 0)
                    {
                        foreach (var item in model.Items)
                        {

                            if (item.GSM != null || item.Description != null)
                            {
                                var JobcardItem = new Joblineitem
                                {
                                    JobCardId = JobCard.JobCardId,
                                    Description = item.Description,
                                    Size = item.Size,
                                    Gsm = item.GSM,
                                    Imp = item.IMP,  // Assuming IMP field is present in 'item'
                                    Side = item.Side,
                                    StartTime = item.StartTime,  // Assuming StartTime field is present in 'item'
                                    EndTime = item.EndTime,  // Assuming EndTime field is present in 'item'
                                    MachineReading = item.MachineReading,  // Assuming MachineReading field is present in 'item'
                                    EndMachineReading = item.EndMachineReading,  // Assuming EndMachineReading field is present in 'item'
                                    TotImp = item.TotIMP,  // Assuming TotIMP field is present in 'item'
                                    PlateNum = item.PlateNum,
                                    CreatedDate= DateTime.Now// Assuming PlateNum field is present in 'item'
                                                             // New property for total values
                                };

                                _context.Joblineitems.Add(JobcardItem); // Add each item to context
                                _context.SaveChanges();

                                TempData["SuccessMessage"] = "Invoice created successfully.";

                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                // Set failure message in TempData
                TempData["ErrorMessage"] = "An error occurred while saving the invoice: " + ex.Message;
            }

            return RedirectToAction("JobCardList");

        }
        [HttpGet]
        public IActionResult JobCardList ()
        {
            var result = from a in _context.Jobcarddetails
                         join b in _context.CustomersDetails
                         on a.CustomerId equals b.CustomerId
                         orderby a.IssueDate descending
                         select new JobCardViewModel
                         {
                             JobCardId = a.JobCardId,
                             IssueDate = a.IssueDate,
                             CustomerName = b.CustomerName,
                             
                         };
            var jobCardInfo = result.ToList();
            return View(jobCardInfo);
        }

        [HttpGet]
        public IActionResult GetJobCardDetails(string jobCardId)
        {
            // Fetch all related data first
            var query = from a in _context.Jobcarddetails
                        join b in _context.CustomersDetails on a.CustomerId equals b.CustomerId
                        join c in _context.Joblineitems on a.JobCardId equals c.JobCardId
                        where a.JobCardId == jobCardId // Optional: Filter by specific JobCardId
                        select new
                        {
                            JobCard = a,
                            Customer = b,
                            LineItem = c
                        };

            // Group the data by JobCardId
            var groupedResult = query.GroupBy(x => new
            {
                x.JobCard.JobCardId,
                x.JobCard.IssueDate,
                x.Customer.CustomerId
            }).Select(g => new JobCardViewModel
            {
                JobCardId = g.Key.JobCardId.ToString(),
                IssueDate = g.Key.IssueDate,
                CustomerId =g.Key.CustomerId,
                
                // Get all customers related to the JobCard
                Customers = _context.CustomersDetails.Select(c => new CustomerDetailsViewModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    
                }).ToList(), // All customer details fetched
                             // Get all job line items related to the JobCard
                Items = g.Select(x => new JobItemViewModel
                {
                    SerialNo = x.LineItem.SerialNo,
                    Description = x.LineItem.Description,
                    Size = x.LineItem.Size,
                    GSM = x.LineItem.Gsm,
                    IMP = x.LineItem.Imp,
                    Side = x.LineItem.Side,
                    StartTime = x.LineItem.StartTime,
                    EndTime = x.LineItem.EndTime,
                    MachineReading = x.LineItem.MachineReading,
                    EndMachineReading = x.LineItem.EndMachineReading,
                    TotIMP = x.LineItem.TotImp,
                    PlateNum = x.LineItem.PlateNum,
                    JobItemId =x.LineItem.JobItemId,
                }).ToList()
            }).FirstOrDefault();

            return PartialView("_JobCardDetailsPartial", groupedResult);
        }
        [HttpPost]
        public IActionResult UpdateJobCard(JobCardViewModel model)
        {
            try
            {
                if (model != null)
                {
                    bool isUpdated = false;
                    var Customer_id_check = _context.Jobcarddetails.Where(jobid => jobid.JobCardId == model.JobCardId).ToList();
                    if (model.JobCardId != Customer_id_check[0].JobCardId)
                    {
                        string updateSql = "UPDATE jobcarddetails " +
                             "SET CustomerId = @CustomerId, IssueDate = @IssueDate " +
                             "WHERE  JobCardId = @JobCardId;";

                        var result = _context.Database.ExecuteSqlRaw(updateSql,
                                                        new MySqlParameter("@CustomerId", model.CustomerId),
                                                        new MySqlParameter("@IssueDate", model.IssueDatevalues),
                                                        new MySqlParameter("@JobCardId", model.JobCardId));

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
                    else
                    {
                        string updateSql = "UPDATE jobcarddetails " +
                             "SET CustomerId = @CustomerId, IssueDate = @IssueDate " +
                             "WHERE  JobCardId = @JobCardId;";

                        var result = _context.Database.ExecuteSqlRaw(updateSql,
                                                        new MySqlParameter("@CustomerId", model.CustomerId),
                                                        new MySqlParameter("@IssueDate", model.IssueDatevalues),
                                                        new MySqlParameter("@JobCardId", model.JobCardId));

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

                        var existingItemIds = _context.Joblineitems
                       .Where(lineItem => lineItem.JobCardId == model.JobCardId)
                       .Select(lineItem => lineItem.JobItemId)
                       .ToList();

                        var pushedItemIds = model.Items.Select(item => item.SerialNo).ToList();

                        var extraItemIds = existingItemIds.Except(pushedItemIds).ToList();

                        if (extraItemIds.Any())
                        {
                            var itemsToDelete = _context.Joblineitems
                                .Where(item => extraItemIds.Contains(item.SerialNo) && item.JobCardId == model.JobCardId)
                                .ToList();

                            _context.Joblineitems.RemoveRange(itemsToDelete);
                            _context.SaveChanges();
                            isUpdated = true;
                        }

                        foreach (var item in model.Items)
                        {
                            var ItemJobIdcheck = _context.Joblineitems
                                   .Where(Itemsvalues => Itemsvalues.JobItemId == item.JobItemId)
                                   .ToList();

                            var jobCardLineItemCheck = _context.Joblineitems.Where(lineItem => lineItem.JobCardId == model.JobCardId).ToList();

                            if (ItemJobIdcheck.Count > 0)
                            {
                                string UpdateJobcardidline = "UPDATE joblineitem " +
                             "SET Description = @Description, Size = @Size, GSM = @GSM, IMP = @IMP, " +
                             "StartTime = @StartTime, EndTime = @EndTime, Side = @Side, " +
                             "MachineReading = @MachineReading, EndMachineReading = @EndMachineReading, PlateNum = @PlateNum, TotIMP = @TotIMP " +
                             "WHERE JobCardId = @JobCardId AND JobItemId = @JobItemId;";

                                _context.Database.ExecuteSqlRaw(UpdateJobcardidline,
                                    new MySqlParameter("@Description", item.Description),
                                    new MySqlParameter("@Size", item.Size),
                                    new MySqlParameter("@GSM", item.GSM),
                                    new MySqlParameter("@IMP", item.IMP),
                                    new MySqlParameter("@StartTime", item.StartTime),
                                    new MySqlParameter("@EndTime", item.EndTime),
                                    new MySqlParameter("@Side", item.Side),
                                    new MySqlParameter("@MachineReading", item.MachineReading),
                                    new MySqlParameter("@EndMachineReading", item.EndMachineReading),
                                    new MySqlParameter("@PlateNum", item.PlateNum),
                                    new MySqlParameter("@TotIMP", item.TotIMP),
                                    new MySqlParameter("@JobCardId", model.JobCardId),
                                    new MySqlParameter("@JobItemId", item.JobItemId));
                                isUpdated = true;
                            }
                            else
                            {
                                string InsertJobcardidline = "INSERT INTO joblineitem " +
                                "(Description, Size, GSM, IMP, StartTime, EndTime, Side,SerialNo, MachineReading, EndMachineReading, PlateNum, JobCardId, TotIMP) " +
                                "VALUES (@Description, @Size, @GSM, @IMP, @StartTime, @EndTime, @Side, @SerialNo, @MachineReading, @EndMachineReading, @PlateNum, @JobCardId, @TotIMP);";

                                _context.Database.ExecuteSqlRaw(InsertJobcardidline,
                                    new MySqlParameter("@Description", item.Description),
                                    new MySqlParameter("@Size", item.Size),
                                    new MySqlParameter("@GSM", item.GSM),
                                    new MySqlParameter("@IMP", item.IMP),
                                    new MySqlParameter("@StartTime", item.StartTime),
                                    new MySqlParameter("@EndTime", item.EndTime),
                                    new MySqlParameter("@Side", item.Side),
                                    new MySqlParameter("@SerialNo","0"),
                                    new MySqlParameter("@MachineReading", item.MachineReading),
                                    new MySqlParameter("@EndMachineReading", item.EndMachineReading),
                                    new MySqlParameter("@PlateNum", item.PlateNum),
                                    new MySqlParameter("@JobCardId", model.JobCardId),
                                    new MySqlParameter("@TotIMP", item.TotIMP)); // Add this parameter
                                isUpdated = true;

                            }
                        }
                    }
                    if (isUpdated)
                    {
                        TempData["SuccessMessage"] = "JobCard updated successfully!";
                    }
                    else
                    {
                        TempData["FailureMessage"] = "No changes were made to the invoice.";
                    }
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                TempData["FailureMessage"] = "An error occurred while updating the invoice. Please try again.";
            }

            // If validation fails, return the view with the model so the user can fix the errors
            return RedirectToAction("JobCardList");
        }

        [HttpPost]
        public IActionResult DeleteJobCard(string id)
        {
            try
            {
                // Perform the deletion logic (e.g., remove from database)
                var jobCard = _context.Jobcarddetails.FirstOrDefault(j => j.JobCardId == id);
                if (jobCard == null)
                {
                    return Json(new { success = false, message = "Job Card not found." });
                }

                _context.Jobcarddetails.Remove(jobCard);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }







    }
}
