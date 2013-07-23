namespace Cherry.Tasks
{
    public static class TaskSettings
    {
        static TaskSettings()
        {
            DEFAULT_PARALLEL_TASKS_FAIL_ON_FIRST_FAILURE = true;
        }

        public static bool DEFAULT_PARALLEL_TASKS_FAIL_ON_FIRST_FAILURE { get; set; }
    }
}
