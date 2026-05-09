<#
.SYNOPSIS
    Prueba de concurrencia para POST /api/v1/reservations.
    Lanza N requests simultáneas intentando reservar el MISMO asiento.
    Espera que solo 1 devuelva 201 y el resto 409.

    Para realizar la prueba ejecutar el siguiente comando en powershell 7 (reemplazando SeatId por un asiento válido):
    ./scripts/stress-test-reservation.ps1 -SeatId "[Colocar SeatId]" -ConcurrentRequests 10
#>

param(
    [string]$ApiUrl = "https://localhost:39716",
    [int]$EventId = 1,
    [Parameter(Mandatory=$true)][string]$SeatId,
    [int]$ConcurrentRequests = 10,
    [int]$UserCount = 10
)

Write-Host "=== Prueba de Concurrencia ===" -ForegroundColor Cyan
Write-Host "API: $ApiUrl"
Write-Host "Event: $EventId  |  Seat: $SeatId"
Write-Host "Lanzando $ConcurrentRequests requests simultaneas con $UserCount usuarios distintos..." -ForegroundColor Yellow
Write-Host ""

$body = @{ eventId = $EventId; seatIds = @($SeatId) } | ConvertTo-Json

# Lanzar requests en paralelo
$results = 1..$ConcurrentRequests | ForEach-Object -Parallel {
    $userId = (($_ - 1) % $using:UserCount) + 1
    $headers = @{
        "Content-Type" = "application/json"
        "X-User-Id"    = "$userId"
    }

    $start = Get-Date
    try {
        $response = Invoke-WebRequest `
            -Uri "$using:ApiUrl/api/v1/reservations" `
            -Method POST `
            -Headers $headers `
            -Body $using:body `
            -SkipCertificateCheck `
            -ErrorAction Stop
        $duration = ((Get-Date) - $start).TotalMilliseconds

        [PSCustomObject]@{
            Request    = $_
            UserId     = $userId
            Status     = $response.StatusCode
            DurationMs = [int]$duration
            Result     = "OK"
        }
    }
    catch {
        $duration = ((Get-Date) - $start).TotalMilliseconds
        $statusCode = $_.Exception.Response.StatusCode.value__
        [PSCustomObject]@{
            Request    = $_
            UserId     = $userId
            Status     = $statusCode
            DurationMs = [int]$duration
            Result     = $_.ErrorDetails.Message
        }
    }
} -ThrottleLimit $ConcurrentRequests

# Reporte
Write-Host "=== Resultados ===" -ForegroundColor Cyan
$results | Sort-Object Request | Format-Table -AutoSize

$success = ($results | Where-Object Status -eq 201).Count
$conflicts = ($results | Where-Object Status -eq 409).Count
$other = $results.Count - $success - $conflicts
$expectedConflicts = $ConcurrentRequests - 1

Write-Host ""
Write-Host "Reservas exitosas (201): $success" -ForegroundColor Green
Write-Host "Conflictos (409):        $conflicts" -ForegroundColor Yellow
Write-Host "Otros errores:           $other" -ForegroundColor Red
Write-Host ""

if ($success -eq 1 -and ($success + $conflicts) -eq $ConcurrentRequests) {
    Write-Host "TEST PASADO: exactamente 1 request gano, los demas fueron rechazados con 409." -ForegroundColor Green
} else {
    $msg = "TEST FALLIDO: se esperaba 1x 201 + " + $expectedConflicts + "x 409."
    Write-Host $msg -ForegroundColor Red
}