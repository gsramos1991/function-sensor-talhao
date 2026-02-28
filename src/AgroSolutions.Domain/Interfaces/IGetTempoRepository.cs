using AgroSolutions.Domain.Dto;

namespace AgroSolutions.Domain.Interfaces
{
    public interface IGetTempoRepository
    {
        Task<GetTempoDto> RequestDadosTempo(string city);
    }
}
