namespace Cherry.Tasks
{
    public delegate TResult ResultCallback<in TParameter, out TResult>(TParameter parameter);

    public delegate TResult ResultCallback<out TResult>();
}