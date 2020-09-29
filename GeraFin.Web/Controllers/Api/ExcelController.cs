using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using GeraFin.Models.Importer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GeraFin.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Excel")]
    public class ExcelController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExcelController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(5000000)]
        public async Task<IActionResult> UploadPriceList(IFormFile ProductUpdates)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\upload\\Excel\\", ProductUpdates.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await ProductUpdates.CopyToAsync(stream);
                }

                FileInfo exFile = new FileInfo(path);
                
                LoggFile(exFile.Name, 0, 0, false, User.Identity.Name, User.Identity.Name);
                
                ProcessFileDetail(exFile);
            }
            catch (Exception Ex)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index");
        }

        public void LoggFile(string FileName, int FileRecordCount, int RecordsProcessed, bool Approved, string UploadedBy, string ApprovedBy)
        {
            UpdateFilesUploaded updateFilesUploaded = new UpdateFilesUploaded
            {
                FileName = FileName,
                FileUploadDate = DateTime.Now,
                FileRecordCount = FileRecordCount,
                FileRecordsProcessed = RecordsProcessed,
                IsApproved = Approved,
                UploadedBy = UploadedBy,
                ApprovedBy = ApprovedBy
            };

            _context.UpdateFilesUploaded.Add(updateFilesUploaded);
            _context.SaveChanges();
        }

        public void ProcessFileDetail(FileInfo exFile)
        {
            try
            {
                using (ExcelPackage package = new ExcelPackage(exFile))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    int TotalRows = workSheet.Dimension.Rows;

                    List<UpdateProductList> Lines = new List<UpdateProductList>();

                    for (int row = 1; row <= TotalRows; row++)
                    {
                        Lines.Add(new UpdateProductList
                        {
                            Unit = workSheet.Cells[row, 1].Value.ToString(),
                            ProductName = workSheet.Cells[row, 2].Value.ToString(),
                            SupplierCode = workSheet.Cells[row, 3].Value.ToString(),
                            Category = workSheet.Cells[row, 4].Value.ToString(),
                            ShortDesc = workSheet.Cells[row, 5].Value.ToString(),
                            Description = workSheet.Cells[row, 6].Value.ToString(),
                            Brand = workSheet.Cells[row, 7].Value.ToString(),
                            UOM = workSheet.Cells[row, 8].Value.ToString(),
                            Supplier = workSheet.Cells[row, 9].Value.ToString(),
                            Price = workSheet.Cells[row, 10].Value.ToString()
                        });
                    }
                    _context.UpdateProductList.AddRange(Lines);
                    _context.SaveChanges();
                }
                RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                RedirectToAction("Error", "Home");
            }
        }

        public void UpdateFile(int FileId, int RecordCount, int RecordsProcessed, bool Approved)
        {
            UpdateFilesUploaded updateFilesUploaded = _context.UpdateFilesUploaded.Where(x => x.FileUploadId == FileId).FirstOrDefault();
            updateFilesUploaded.FileRecordCount = RecordCount;
            updateFilesUploaded.FileRecordsProcessed = RecordsProcessed;
            updateFilesUploaded.IsApproved = Approved;

            _context.UpdateFilesUploaded.Update(updateFilesUploaded);
            _context.SaveChanges();
        }
    }
}