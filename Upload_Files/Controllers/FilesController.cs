using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Upload_Files.Data;
using Upload_Files.Models;
using Upload_Files.ViewModels;

namespace Upload_Files.Controllers
{
	public class FilesController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
		{
			return View(_context.UploadedFiles);
		}

        public IActionResult Upload()
        {
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UploadFiles(UploadFilesFormViewModel model)
		{
			// Vaildate files extenstions and size

			List<UploadedFile> uploadedFiles = new();

            foreach (var file in model.Files)
            {
				var fakeFileName = Path.GetRandomFileName();
				UploadedFile uploadedFile = new UploadedFile()
				{
					FileName = file.FileName,
					ContentType = file.ContentType,
					StoredFileName = fakeFileName
                };

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fakeFileName);

                using FileStream fileStream = new(path, FileMode.Create);
                file.CopyTo(fileStream);

                uploadedFiles.Add(uploadedFile);
            }

            _context.AddRange(uploadedFiles);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
		}

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            var uploadedFile = _context.UploadedFiles.SingleOrDefault(f => f.StoredFileName == fileName);

            if (uploadedFile is null)
                return NotFound();

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            MemoryStream memoryStream = new();
            using FileStream fileStream = new(path, FileMode.Open);
            fileStream.CopyTo(memoryStream);

            memoryStream.Position = 0;

            return File(memoryStream, uploadedFile.ContentType, uploadedFile.FileName);
        }
    }
}
