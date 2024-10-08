using Microsoft.SemanticKernel;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;
using Dapper;

namespace SemanticKernel.Plugins;

public class EmployeeSalary
{
    public required string Name { get; set; }
    public required string MonthlySalary { get; set; }
    public required string Company { get; set; }
}



public class SalariesPlugin
{
    [KernelFunction("get_employee_salaries_data")]
    [Description("""
            Gets a list of employee salaries. The salaries are store in a mysql database in a table called salariosSv. The table Schema is as follow:
            - Empleado: varchar (which is the name of the employee)
            - Salario: decimal (which is the monthly salary of the employee)
            - Empresa: varchar (which is the company the employee works for)      
            
            The name of the employee two lastname(family names) follow by two given name, users normally will ask for employee by a single given name and a single surname.
            e.g: Rafael Romero, in the db that name is store as Romero Rodriguez.

            The company name is store as the registered name, for example:
            Bitworks SA de CV but the user will ask for Bitworks, or TACA International Airlines, S.A. de C.V. but the user will ask for TACA. 
            So, please consider using likes if neccesary.

            please write the where and/or orderby clause. I expect something like WHERE ....

            """)]
    [return: Description("An array of employee salaries information")]
    public async Task<IEnumerable<EmployeeSalary>> GetEmployeeSalariesData([Description("The where and/or orderby for my sql")] string whereOrderClause)
    {
        using var cnn = await GetDbConnectionAsync();
        var sql = $"SELECT Empleado AS Name, Salario AS MonthlySalary, Empresa AS Company FROM salariosSv {whereOrderClause}";
        return await cnn.QueryAsync<EmployeeSalary>(sql);
    }

    [KernelFunction("summarize_data")]
    [Description("""
         The salaries are store in a mysql database in a table called salariosSv. The table Schema is as follow:
        - Empleado: varchar (which is the name of the employee)
        - Salario: decimal (which is the monthly salary of the employee)
        - Empresa: varchar (which is the company the employee works for)
        
        Please write the query to summarize the data. I expect something like SELECT SUM(Salario) FROM salariosSv WHERE ....
        or SELECT AVG(Salario) FROM salariosSv WHERE .... or SELECT COUNT(*) FROM salariosSv WHERE ....

        The query you write should return a single numeric value.

        """)]
    public async Task<decimal> GetSummarizedData([Description("The query to summarize the data")] string query)
    {
        var cnn = await GetDbConnectionAsync();
        return await cnn.ExecuteScalarAsync<decimal>(query);
    } 

    private async Task<IDbConnection> GetDbConnectionAsync()
    {
        var connectionString = "Server=localhost;Port=4407;Database=SalariosSv;User=root;Password=password;";
        var cnn = new MySqlConnection(connectionString);
        await cnn.OpenAsync();
        return cnn;
    }
}
