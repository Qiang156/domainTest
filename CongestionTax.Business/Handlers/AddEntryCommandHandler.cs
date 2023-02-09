using CongestionTax.Business.Commands;
using CongestionTax.DataLayer.Repositories;
using CongestionTax.Domain;
using CongestionTax.Domain.Exceptions;
using CongestionTax.Domain.Extensions;
using CongestionTax.Domain.Services;
using MediatR;

namespace CongestionTax.Business.Handlers;

public class AddEntryCommandHandler : IRequestHandler<AddEntryCommand>
{
    private readonly IDailyFeeRepository _dailyFeeRepository;
    private readonly IRuleRepository _ruleRepository;
    private readonly IHolidayService _holidayService;

    public AddEntryCommandHandler(IDailyFeeRepository dailyFeeRepository, IRuleRepository ruleRepository,
        IHolidayService holidayService)
    {
        _dailyFeeRepository = dailyFeeRepository;
        _ruleRepository = ruleRepository;
        _holidayService = holidayService;
    }

    public async Task<Unit> Handle(AddEntryCommand request, CancellationToken cancellationToken)
    {
        var currentTime = DateTime.Now;

        var reference = request.RegistrationNumber.GenerateDailyFeeReference(request.EntryTime ?? currentTime, request.City);

        var record =
            _dailyFeeRepository.Get(reference);

        if (record == null)
        {
            var rule = _ruleRepository.Get(request.City);

            if (rule == null)
            {
                throw new RuleNotFoundException($"Rule Not Found For City: {request.City}");
            }

            record = DailyFee.Create(rule, request.RegistrationNumber, request.Vehicle,
                request.EntryTime ?? currentTime, request.City);
        }

        await record.AddEntry(request.EntryTime ?? currentTime, _holidayService);

        _dailyFeeRepository.Upsert(record);

        return new Unit();
    }
}