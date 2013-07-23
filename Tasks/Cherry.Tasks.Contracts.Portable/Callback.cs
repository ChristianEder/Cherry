namespace Cherry.Tasks
{
    public delegate void Callback<in TParameter>(TParameter parameter);

    public delegate void Callback();
}