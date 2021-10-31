// vi: filetype=cs
open System

type Expense(expenseType: ExpenseType, amount: int) =
    member this.ExpenseType = expenseType
    member this.Amount = amount
    with
        static member Breakfast amount = Expense(BREAKFAST, amount)
        static member Lunch amount = Expense(LUNCH, amount)
        static member Dinner amount = Expense(DINNER, amount)
        static member CarRental amount = Expense(CAR_RENTAL, amount)

        member this.GetName() =
            match this.ExpenseType with
            | DINNER -> "Dinner"
            | BREAKFAST -> "Breakfast"
            | CAR_RENTAL -> "Car Rental"
            | LUNCH -> "Lunch"

        member private this.GetLimit() =
            match this.ExpenseType with
            | BREAKFAST -> Some 1000
            | LUNCH -> Some 2000
            | DINNER -> Some 5000
            | _ -> None

        member this.GetIsOverLimit() =
            let limit = this.GetLimit()
            limit.IsSome && this.Amount > limit.Value

        member this.IsMeal() =
            let mealTypes = [ DINNER; BREAKFAST; LUNCH ]
            Seq.contains this.ExpenseType mealTypes

and ExpenseType =
    | DINNER
    | BREAKFAST
    | CAR_RENTAL
    | LUNCH

type ExpenseReport =
    { Date: DateTime
      MealExpenses: int
      TotalExpenses: int
      Entries: ExpenseReportEntry seq }

and ExpenseReportEntry =
    { Name: string
      Amount: int
      IsOverLimit: bool }

type ExpenseReport with
    static member Create(expenses: seq<Expense>) : ExpenseReport =
        let getExpenseReportEntry (expense: Expense) =
            { Name = expense.GetName()
              Amount = expense.Amount
              IsOverLimit = expense.GetIsOverLimit() }

        let getAmount (expense: Expense) = expense.Amount
        let getExpenseSum = Seq.sumBy getAmount

        let getMealExpenseSum (expenses: Expense seq) =
            expenses
            |> Seq.filter (fun x -> x.IsMeal())
            |> getExpenseSum

        { Date = DateTime.Now
          MealExpenses = getMealExpenseSum expenses
          TotalExpenses = getExpenseSum expenses
          Entries = Seq.map getExpenseReportEntry expenses }

let printReport (expenses: seq<Expense>) =
    let expenseReport = ExpenseReport.Create(expenses)
    printfn "Expense Report: %s" (expenseReport.Date.ToString())

    for expense in expenseReport.Entries do
        let mealOverExpensesMarker = if expense.IsOverLimit then "X" else " "
        printfn "%s\t%d\t%s" expense.Name expense.Amount mealOverExpensesMarker

    printfn "Meal Expenses: %d" expenseReport.MealExpenses
    printfn "Total Expenses: %d" expenseReport.TotalExpenses

[<EntryPoint>]
let main argv =
    printReport [ Expense.Dinner(5000)
                  Expense.Dinner(5001)
                  Expense.Breakfast(1000)
                  Expense.Breakfast(1001)
                  Expense.CarRental(4) ]

    0
