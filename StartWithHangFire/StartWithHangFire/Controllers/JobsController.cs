using Hangfire;
using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using StartWithHangFire.Service;
using System;
using System.Threading.Tasks;

namespace StartWithHangFire.Controllers
{
    [ApiController]
    public class JobsController : Controller
    {
       private readonly IJobToProcess _jobToProcess;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recuringJobManager;
        
        public JobsController(IBackgroundJobClient backgroundJobs,
                              IRecurringJobManager recuringJobManager,
                              IJobToProcess jobToProcess)
        {
            _recuringJobManager = recuringJobManager;
            _backgroundJobs = backgroundJobs;
            _jobToProcess = jobToProcess;
        }

        [HttpGet("jobs/home")]
        public IActionResult Index()
        {
            return Ok("Api Web HangFire running...");
        }

        /// <summary>
        /// Fire and Forget jobs are executed only once and almost immediatelly after creation
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/fire-and-forget")]
        public async Task<IActionResult> FireAndForget()
        {
            Console.WriteLine($"Request: {DateTime.Now}");

            //Inserindo em uma fila específica
            _backgroundJobs.Create<IJobToProcess>(job => job.InsertUser("FireAndForget"), new EnqueuedState("mainfila1"));

            //var jobFireForget = _backgroundJobs.Enqueue<IJobToProcess>(job => job.InsertUser("FireAndForget"));
            return Ok("Job Criado com Sucesso");
        }

        /// <summary>
        /// Job Delayed are executed only once too, but not immediately. It takes a certain time interval.
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/job-delayed")]
        public IActionResult JobDelayed()
        {
            Console.WriteLine($"Request: {DateTime.Now}");
            var jobDelayed = _backgroundJobs.Schedule<IJobToProcess>(job =>
                                                    job.InsertUser($"JobDelayed"),
                                                    TimeSpan.FromSeconds(30));

            ContinueWith(jobDelayed);

            return Ok("Job Criado com Sucesso");
        }

        /// <summary>
        /// Continuations are executed when its parent Id has been finished
        /// </summary>
        /// <returns></returns>
        private string ContinueWith(string jobId)
        {
            Console.WriteLine($"Request: {DateTime.Now}");

            //jobId é o id do serviço que o método aguardará para começar a ser executado.
            _backgroundJobs.ContinueJobWith<IJobToProcess>(jobId, job => job.InsertUser("ContinuedWith"));

            return "Job Criado com Sucesso";
        }

        /// <summary>
        /// Recurring jobs are fire many times on the specific CRON schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/recuring-job")]
        public IActionResult RecurringJobAddOrUpdate()
        {
            RecurringJob.AddOrUpdate<IJobToProcess>(
                                                methodCall: job => job.InsertUser("RecurringJob-AddOrUpdate"),
                                                cronExpression: Cron.Minutely);

            return Ok("Job Criado com Sucesso");
        }


        [HttpGet("jobs/recuring-job2")]
        public IActionResult RecurringJobManager()
        {
            _recuringJobManager.AddOrUpdate<IJobToProcess>(
                                                recurringJobId: Guid.NewGuid().ToString(),
                                                methodCall: job => job.InsertUser("RecurringJob2-AddOrUpdate"),
                                                cronExpression: Cron.Minutely,
                                                queue: "mainfila1");

            return Ok("Recuring-job2 Criado com Sucesso");
        }

        [HttpGet("jobs/exception-job")]
        public IActionResult ExceptionJob()
        {
            BackgroundJob.Enqueue<IJobToProcess>(x =>
                x.JobException("Enqueue Job And Throw Exception"));

            return Ok("Job Criado com Sucesso");
        }

        [HttpGet("jobs/enqueue")]
        public IActionResult Enqueue()
        {
            BackgroundJob.Enqueue<IJobToProcess>(x =>
                x.InsertUser("Enqueued Job To save an User"));

            return Ok("Job Criado com Sucesso");
        }

    }
}
