// vi: filetype=cs
open System

module Expense =
    type T(expenseType: ExpenseType, amount: int) =
        member this.ExpenseType = expenseType
        member this.Amount = amount

    and ExpenseType =
        | DINNER
        | BREAKFAST
        | CAR_RENTAL
        | LUNCH

    let Breakfast amount = T(BREAKFAST, amount)
    let Lunch amount = T(LUNCH, amount)
    let Dinner amount = T(DINNER, amount)
    let CarRental amount = T(CAR_RENTAL, amount)

    let getName (expense: T) =
        match expense.ExpenseType with
        | DINNER -> "Dinner"
        | BREAKFAST -> "Breakfast"
        | CAR_RENTAL -> "Car Rental"
        | LUNCH -> "Lunch"

    let private getLimit (expense: T) =
        match expense.ExpenseType with
        | BREAKFAST -> Some 1000
        | LUNCH -> Some 2000
        | DINNER -> Some 5000
        | _ -> None

    let isOverLimit (expense: T) =
        let limit = getLimit expense
        limit.IsSome && expense.Amount > limit.Value

    let isMeal (expense: T) =
        let mealTypes = [ DINNER; BREAKFAST; LUNCH ]
        Seq.contains expense.ExpenseType mealTypes

module ExpenseReport =
    type T =
        { Date: DateTime
          MealExpenses: int
          TotalExpenses: int
          Entries: Entry seq }

    and Entry =
        { Name: string
          Amount: int
          IsOverLimit: bool }

    let private getEntry (expense: Expense.T) =
        { Name = Expense.getName expense
          Amount = expense.Amount
          IsOverLimit = Expense.isOverLimit expense }

    let private getAmount (expense: Expense.T) = expense.Amount
    let private getExpenseSum = Seq.sumBy getAmount

    let private getMealExpenseSum =
        Seq.filter Expense.isMeal >> getExpenseSum

    let create (expenses: Expense.T seq) (date: DateTime) : T =
        { Date = date
          MealExpenses = getMealExpenseSum expenses
          TotalExpenses = getExpenseSum expenses
          Entries = Seq.map getEntry expenses }

    let print (expenses: Expense.T seq) =
        let expenseReport = create expenses DateTime.Now

        printfn "Expense Report: %s" (expenseReport.Date.ToString())

        for expense in expenseReport.Entries do
            let mealOverExpensesMarker = if expense.IsOverLimit then "X" else " "
            printfn "%s\t%d\t%s" expense.Name expense.Amount mealOverExpensesMarker

        printfn "Meal Expenses: %d" expenseReport.MealExpenses
        printfn "Total Expenses: %d" expenseReport.TotalExpenses

[<EntryPoint>]
let main argv =
    ExpenseReport.print [ Expense.Dinner(5000)
                          Expense.Dinner(5001)
                          Expense.Breakfast(1000)
                          Expense.Breakfast(1001)
                          Expense.CarRental(4) ]

    0
