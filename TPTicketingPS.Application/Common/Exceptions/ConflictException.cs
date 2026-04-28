
/// <summary>
/// Operación inválida por estado actual del recurso. Se traduce a 409 Conflict.
/// Casos típicos: email duplicado, asiento ya reservado, reserva ya pagada.
/// </summary>
namespace TPTicketingPS.Application.Common.Exceptions;

public class ConflictException(string message) : Exception(message);
