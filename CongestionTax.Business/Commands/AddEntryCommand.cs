using CongestionTax.Domain;
using MediatR;

namespace CongestionTax.Business.Commands;

public record AddEntryCommand
    (string RegistrationNumber, Vehicle Vehicle, string City, DateTime? EntryTime = null) : IRequest;