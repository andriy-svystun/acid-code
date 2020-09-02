using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AcidCode.Common;
using AcidCode.Core;
using AcidCode.Db;
using AcidCode.Web.Models;

namespace AcidCode.Web.Controllers
{
    public class CodeItemController : Controller
    {
        private readonly IAcidCodeRepository _repository;
        private readonly IAcidCodeProcessor _codeProcessor;

        public CodeItemController(IAcidCodeRepository repository, IAcidCodeProcessor codeProcessor)
        {
            _repository = repository;
            _codeProcessor = codeProcessor;
        }

        // GET: CodeItem
        public async Task<ActionResult> Get(int id)
        {
            var codeitem = await _repository.GetCodeItemAsync(id);

            if (codeitem == null)
                throw new HttpException(404, "Item not found");

            return View(codeitem);
        }

        public ActionResult Index()
        {
            ViewBag.CurrentPage = "List";
            return View(_repository.CodeItems.ToList());
        }

        public async Task<ActionResult> RunCode(int id, string returnurl)
        {
            CodeRunResultViewModel viewModel = new CodeRunResultViewModel();

            var codeitem = await _repository.GetCodeItemAsync(id);

            ViewBag.ReturnUrl = returnurl;

            if (codeitem == null)
                throw new HttpException(404, "Item not found");

            var codecompiler = await _codeProcessor.GetCodeCompilerFromText(codeitem.CodeText);

            await codecompiler.CompileCodeAsync();

            if (!codecompiler.IsCodeCompiled)
            {
                viewModel.IsSucceeded = false;
                viewModel.ErrorsText = codecompiler.CompilationErrors;
                return View("CodeRunResult", viewModel);
            }

            var coderun = await _codeProcessor.GetCodeRunner(codecompiler);

            await coderun.RunCodeAsync();

            viewModel.IsSucceeded = String.IsNullOrEmpty(coderun.LastErrors);
            viewModel.RunningTime = coderun.ExecutionTime.ToString();
            viewModel.CodeOutput = coderun.CodeOutput;
            viewModel.ErrorsText = coderun.LastErrors;

            return View("CodeRunResult", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> EditCode(int id)
        {
            var codeitem = await _repository.GetCodeItemAsync(id);

            ViewBag.ReturnUrl = Url.Action("Get", new { id = id });

            if (codeitem == null)
                throw new HttpException(404, "Item not found");

            return View(codeitem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> EditCode(CodeItem codeItem, string returnurl)
        {
            if (ModelState.IsValid)
            {
                if (codeItem.Id == 0)
                {
                    CodeRunResultViewModel viewModel = new CodeRunResultViewModel();
                    var codetext = codeItem.CodeText.Replace("\r", "").Replace("\n", "");
                    var codecompiler = await _codeProcessor.GetCodeCompilerFromText(codetext);

                    codecompiler.CompileCode();

                    if (!codecompiler.IsCodeCompiled)
                    {
                        viewModel.IsSucceeded = false;
                        viewModel.ErrorsText = codecompiler.CompilationErrors;
                        return View("CodeRunResult", viewModel);
                    }

                    var result = await _codeProcessor.SaveCompiledCode(codecompiler);
                }
                else
                {
                    await _repository.SaveCodeItemAsync(codeItem);
                }
            
                
                return returnurl != null ? Redirect(returnurl) : Redirect( Url.Action("Index", "CodeItem")); 
            }

            return View(codeItem);
        }

        [HttpGet]
        public ActionResult AddCode()
        {
            CodeItem codeItem = new CodeItem();

            ViewBag.ReturnUrl = Url.Action("Index", "CodeItem");

            ViewBag.Title = "Add code";

            return View("EditCode", codeItem);
        }
    }
}