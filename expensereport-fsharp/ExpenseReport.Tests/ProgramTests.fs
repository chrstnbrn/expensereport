module Tests

open System
open Program
open Xunit

[<Fact>]
let ``Report has current date`` () =
    let expenses = Seq.empty

    let actual = getExpenseReport expenses

    Assert.True(Math.Abs((actual.Date - DateTime.Now).TotalMilliseconds) < 10.0)

[<Fact>]
let ``Report is empty if there are no expenses`` () =
    let expenses = Seq.empty

    let actual = getExpenseReport expenses

    Assert.Empty(actual.Expenses)
    Assert.Equal(0, actual.MealExpenses)
    Assert.Equal(0, actual.TotalExpenses)

[<Fact>]
let ``Report for multiple expenses`` () =
    let expenses =
        [ Expense(BREAKFAST, 100)
          Expense(DINNER, 200)
          Expense(DINNER, 300)
          Expense(CAR_RENTAL, 400)
          Expense(LUNCH, 500) ]

    let actual = getExpenseReport expenses

    Assert.Equal<ExpenseReportExpense>(
        [| { Name = "Breakfast"
             Amount = 100
             IsOverLimit = false }
           { Name = "Dinner"
             Amount = 200
             IsOverLimit = false }
           { Name = "Dinner"
             Amount = 300
             IsOverLimit = false }
           { Name = "Car Rental"
             Amount = 400
             IsOverLimit = false }
           { Name = "Lunch"
             Amount = 500
             IsOverLimit = false } |],
        actual.Expenses
    )

    Assert.Equal(1100, actual.MealExpenses)
    Assert.Equal(1500, actual.TotalExpenses)

[<Fact>]
let ``Report does not show breakfast for 1000 as over limit`` () =
    let expenses = [ Expense(BREAKFAST, 1000) ]

    let actual = getExpenseReport expenses

    Assert.False((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report shows breakfast for 1001 as over limit`` () =
    let expenses = [ Expense(BREAKFAST, 1001) ]

    let actual = getExpenseReport expenses

    Assert.True((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report does not show dinner for 5000 as over limit`` () =
    let expenses = [ Expense(DINNER, 5000) ]

    let actual = getExpenseReport expenses

    Assert.False((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report shows dinner for 5001 as over limit`` () =
    let expenses = [ Expense(DINNER, 5001) ]

    let actual = getExpenseReport expenses

    Assert.True((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report does not show lunch for 2000 as over limit`` () =
    let expenses = [ Expense(LUNCH, 2000) ]

    let actual = getExpenseReport expenses

    Assert.False((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report shows lunch for 2001 as over limit`` () =
    let expenses = [ Expense(LUNCH, 2001) ]

    let actual = getExpenseReport expenses

    Assert.True((Seq.head actual.Expenses).IsOverLimit)

[<Fact>]
let ``Report does not show car rental for 1 mio as over limit`` () =
    let expenses = [ Expense(CAR_RENTAL, 1_000_000) ]

    let actual = getExpenseReport expenses

    Assert.False((Seq.head actual.Expenses).IsOverLimit)
