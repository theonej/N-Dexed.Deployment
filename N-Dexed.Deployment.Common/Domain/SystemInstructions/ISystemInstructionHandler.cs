namespace N_Dexed.Deployment.Common.Domain.SystemInstructions
{
    /// <summary>
    /// Objects that implement this interface typically reside in the business layer of the application
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISystemInstructionHandler<T> where T:class, ISystemInstruction
    {
        void Handle(T instruction);
    }
}
