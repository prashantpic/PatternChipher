using Unity.Jobs;
using Unity.Collections; // Potentially needed by jobs, though not directly by this utility
using Unity.Burst; // Potentially needed by jobs, though not directly by this utility

namespace PatternCipher.Services.Utilities
{
    public static class BurstDispatchUtility
    {
        /// <summary>
        /// Schedules a job and immediately completes it.
        /// This is a synchronous operation that blocks until the job is finished.
        /// </summary>
        /// <typeparam name="T">The type of the job, must be an IJob struct.</typeparam>
        /// <param name="jobData">The job data to schedule.</param>
        /// <param name="dependsOn">The JobHandle on which this job depends.</param>
        /// <returns>The JobHandle for the completed job.</returns>
        public static JobHandle ScheduleAndComplete<T>(T jobData, JobHandle dependsOn = default) where T : struct, IJob
        {
            JobHandle handle = jobData.Schedule(dependsOn);
            handle.Complete(); // This blocks the calling thread until the job is done.
            return handle;
        }

        // Add other useful Job System utilities here if needed, e.g.,
        // - Asynchronous scheduling and completion handling via Task
        // - Batch scheduling
    }
}