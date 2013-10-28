namespace Cherry.IoC.Contracts.Portable
{
    /// <summary>
    /// Interface for classes that contain startup code like a bunch of
    /// service registrations that have to be deployed to a <see cref="IServiceRegistry"/>.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Forces this <see cref="IModule"/> instance to load its content into the
        /// given <paramref name="registry"/>
        /// </summary>
        /// <param name="registry">The <see cref="IServiceRegistry"/> to load this module into</param>
        void Load(IServiceRegistry registry);
    }
}
