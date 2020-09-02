using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcidCode.Web.Models;
using AcidCode.Core;
using AcidCode.Common;
using System.Threading.Tasks;

namespace AcidCode.Web.Controllers
{
    public class HomeController : Controller
    {
        IAcidCodeProcessor _codeRunner;

        public HomeController(IAcidCodeProcessor codeRunner)
        {
            _codeRunner = codeRunner;
        }

        // GET: Home
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "Home";
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SendCode(HomeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CodeRunResultViewModel resultViewModel = new CodeRunResultViewModel();
                ViewBag.ReturnUrl = Url.Action("Index");

                try
                {
                    var codetext = viewModel.CodeText.Replace("\r", "").Replace("\n", "");

                    var compiler = await _codeRunner.GetCodeCompilerFromText(viewModel.CodeText);

                    await compiler.CompileCodeAsync();

                    if (!compiler.IsCodeCompiled)
                    {
                        resultViewModel.IsSucceeded = false;
                        resultViewModel.ErrorsText = compiler.CompilationErrors;
                        return View("AnalyzerResult", resultViewModel);
                    }

                    //var codeItem = await _codeRunner.SaveCompiledCode(compiler);

                    var coderun = await  _codeRunner.GetCodeRunner(compiler);

                    await coderun.RunCodeAsync();

                    resultViewModel.IsSucceeded = String.IsNullOrEmpty(coderun.LastErrors);
                    resultViewModel.RunningTime = coderun.ExecutionTime.ToString();
                    resultViewModel.CodeOutput = coderun.CodeOutput;
                    resultViewModel.ErrorsText = coderun.LastErrors;
                }
                catch(Exception ex)
                {
                    resultViewModel.IsSucceeded = false;
                    resultViewModel.ErrorsText = ex.Message;

                }

                return View("AnalyzerResult", resultViewModel);
            }
            else
            {
                return View("Index");
            }
        }
    }
}