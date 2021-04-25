using Hangfire;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using System;

namespace StartWithHangFire.Service
{
    public static class JobsStart
    {
        public static void StartFireAndForget(this IApplicationBuilder app, IBackgroundJobClient backgroundJobs)
        {
            FireAndForget(backgroundJobs);
        }

        public static void FireAndForget(IBackgroundJobClient backgroundJobs)
        {
            Console.WriteLine($"Request: {DateTime.Now}");

            //Inserindo em uma fila específica
            backgroundJobs.Create<IJobToProcess>(job => job.InsertUser("FireAndForget"), new EnqueuedState("mainfila1"));

            //var jobFireForget = _backgroundJobs.Enqueue<IJobToProcess>(job => job.InsertUser("FireAndForget"));
        }
    }
}
