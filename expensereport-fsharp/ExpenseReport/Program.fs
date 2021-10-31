// vi: filetype=cs
open System

type ExpenseType =
    | DINNER
    | BREAKFAST
    | CAR_RENTAL
    | LUNCH

type Expense(expenseType: ExpenseType, amount: int) =
    member this.expenseType = expenseType
    member this.amount = amount

type ExpenseReportExpense =
    { Name: string
      Amount: int
      IsOverLimit: bool }

type ExpenseReport =
    { Date: DateTime
      MealExpenses: int
      TotalExpenses: int
      Expenses: ExpenseReportExpense seq }

let getExpenseName (expense: Expense) =
    match expense.expenseType with
    | DINNER -> "Dinner"
    | BREAKFAST -> "Breakfast"
    | CAR_RENTAL -> "Car Rental"
    | LUNCH -> "Lunch"

let getLimit expenseType =
    match expenseType with
    | BREAKFAST -> Some 1000
    | LUNCH -> Some 2000
    | DINNER -> Some 5000
    | _ -> None

let getIsOverLimit (expense: Expense) =
    let limit = getLimit expense.expenseType
    limit.IsSome && expense.amount > limit.Value

let getExpenseReportExpense (expense: Expense) =
    { Name = getExpenseName expense
      Amount = expense.amount
      IsOverLimit = getIsOverLimit expense }

let isMealExpense (expense: Expense) =
    let mealTypes = [ DINNER; BREAKFAST; LUNCH ]
    Seq.contains expense.expenseType mealTypes

let getAmount (expense: Expense) = expense.amount
let getExpenseSum = Seq.sumBy getAmount

let getMealExpenseSum =
    Seq.filter isMealExpense >> getExpenseSum

let getExpenseReport (expenses: seq<Expense>) : ExpenseReport =
    { Date = DateTime.Now
      MealExpenses = getMealExpenseSum expenses
      TotalExpenses = getExpenseSum expenses
      Expenses = Seq.map getExpenseReportExpense expenses }

let printReport (expenses: seq<Expense>) =
    let expenseReport = getExpenseReport expenses
    printfn "Expense Report: %s" (expenseReport.Date.ToString())

    for expense in expenseReport.Expenses do
        let mealOverExpensesMarker = if expense.IsOverLimit then "X" else " "
        printfn "%s\t%d\t%s" expense.Name expense.Amount mealOverExpensesMarker

    printfn "Meal Expenses: %d" expenseReport.MealExpenses
    printfn "Total Expenses: %d" expenseReport.TotalExpenses

[<EntryPoint>]
let main argv =
    printReport [ Expense(DINNER, 5000)
                  Expense(DINNER, 5001)
                  Expense(BREAKFAST, 1000)
                  Expense(BREAKFAST, 1001)
                  Expense(CAR_RENTAL, 4) ]

    0
