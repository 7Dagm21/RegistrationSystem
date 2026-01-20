# Test SQL Server Connection Script
# This script helps verify your database connection is working

param(
    [string]$Server = "(localdb)\mssqllocaldb",
    [string]$Database = "AASTURegistrationDB"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SQL Server Connection Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test connection using sqlcmd
Write-Host "Testing connection to: $Server" -ForegroundColor Yellow
Write-Host "Database: $Database" -ForegroundColor Yellow
Write-Host ""

try {
    # Test if server is accessible
    $testQuery = "SELECT @@VERSION as Version"
    $result = sqlcmd -S $Server -Q $testQuery -W -h -1 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Successfully connected to SQL Server!" -ForegroundColor Green
        Write-Host ""
        
        # Check if database exists
        $dbCheck = "IF DB_ID('$Database') IS NOT NULL SELECT 'EXISTS' ELSE SELECT 'NOT_FOUND'"
        $dbResult = sqlcmd -S $Server -Q $dbCheck -W -h -1
        
        if ($dbResult -match "EXISTS") {
            Write-Host "✓ Database '$Database' exists" -ForegroundColor Green
            
            # Check tables
            Write-Host ""
            Write-Host "Checking tables..." -ForegroundColor Yellow
            $tableQuery = "USE [$Database]; SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
            $tableCount = sqlcmd -S $Server -d $Database -Q $tableQuery -W -h -1
            
            if ($tableCount -match "\d+") {
                Write-Host "✓ Found tables in database" -ForegroundColor Green
            }
        } else {
            Write-Host "⚠ Database '$Database' does not exist yet" -ForegroundColor Yellow
            Write-Host "  It will be created automatically when you run the application" -ForegroundColor Yellow
        }
        
    } else {
        Write-Host "✗ Failed to connect to SQL Server" -ForegroundColor Red
        Write-Host ""
        Write-Host "Troubleshooting:" -ForegroundColor Yellow
        Write-Host "1. Make sure SQL Server is running" -ForegroundColor White
        Write-Host "2. Check if the server name is correct: $Server" -ForegroundColor White
        Write-Host "3. Try different server names:" -ForegroundColor White
        Write-Host "   - (localdb)\mssqllocaldb (LocalDB)" -ForegroundColor Gray
        Write-Host "   - .\SQLEXPRESS (SQL Express)" -ForegroundColor Gray
        Write-Host "   - localhost (Local SQL Server)" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure sqlcmd is available:" -ForegroundColor Yellow
    Write-Host "  sqlcmd comes with SQL Server installation" -ForegroundColor White
    Write-Host "  Or install SQL Server Command Line Utilities" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
