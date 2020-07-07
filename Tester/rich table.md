
# Rich Table
Function imports types:&nbsp;
 |  **Type** |  **Notes**
| - | - |
 | StoredProcedure&nbsp; | - Return sets using params.&nbsp;- Pass and return params.&nbsp;- Return multiple result sets.&nbsp;
 | TableValuedFunction&nbsp; | Return  **complex** types.&nbsp;
 | ComposableScalarValuedFunction&nbsp; | - <span style='font-weight:bold;text-decoration:underline'>Can only be used** **in LINQ to Entities queries**.&nbsp;- Its body will never be executed. (No body).&nbsp;-  **Cannot be called directly.**
 | NonComposableScalarValuedFunction&nbsp; | - <span style='font-weight:bold;text-decoration:underline'>Cant be used** **in LINQ to Entities queries**.&nbsp;-  **Can be called directly.**- Have body.&nbsp;
 | AggregateFunction&nbsp; | Should use one&nbsp;  **IEnumerable&lt;T&gt; or IQueryable&lt;T&gt;** parameter.&nbsp;
 | BuiltInFunction&nbsp; | Used with RDBMS buildin functions.&nbsp;
 | NiladicFunction&nbsp; | See <a href="https://docs.microsoft.com/en-us/sql/t-sql/statements/create-table-transact-sql?view=sql-server-2017">https://docs.microsoft.com/en-us/sql/t-sql/statements/create-table-transact-sql?view=sql-server-2017</a>&nbsp;
 | ModelDefinedFunction&nbsp; | A Model Defined Function that provides an  **entity sql impl** as well as a **code implementation**.&nbsp;Uses the code when called directly.&nbsp;Uses the entity sql when called in a LINQ query.&nbsp;<a href="https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/entity-sql-overview">https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/entity-sql-overview</a>&nbsp;