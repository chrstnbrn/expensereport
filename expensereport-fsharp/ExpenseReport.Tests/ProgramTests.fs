module Tests

open System
open Program
open Xunit

[<Fact>]
let ``Report has the specified date`` () =
    let expenses = Seq.empty
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.Equal(date, actual.Date)

[<Fact>]
let ``Report is empty if there are no expenses`` () =
    let expenses = Seq.empty
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.Empty(actual.Entries)
    Assert.Equal(0, actual.MealExpenses)
    Assert.Equal(0, actual.TotalExpenses)

[<Fact>]
let ``Report for multiple expenses`` () =
    let expenses =
        [ Expense.Breakfast(100)
          Expense.Dinner(200)
          Expense.Dinner(300)
          Expense.CarRental(400)
          Expense.Lunch(500) ]

    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    let expectedEntries: ExpenseReport.Entry array =
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
             IsOverLimit = false } |]

    Assert.Equal(expectedEntries, actual.Entries)
    Assert.Equal(1100, actual.MealExpenses)
    Assert.Equal(1500, actual.TotalExpenses)

[<Fact>]
let ``Report does not show breakfast for 1000 as over limit`` () =
    let expenses = [ Expense.Breakfast(1000) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.False((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report shows breakfast for 1001 as over limit`` () =
    let expenses = [ Expense.Breakfast(1001) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.True((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report does not show dinner for 5000 as over limit`` () =
    let expenses = [ Expense.Dinner(5000) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.False((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report shows dinner for 5001 as over limit`` () =
    let expenses = [ Expense.Dinner(5001) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.True((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report does not show lunch for 2000 as over limit`` () =
    let expenses = [ Expense.Lunch(2000) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.False((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report shows lunch for 2001 as over limit`` () =
    let expenses = [ Expense.Lunch(2001) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.True((Seq.head actual.Entries).IsOverLimit)

[<Fact>]
let ``Report does not show car rental for 1 mio as over limit`` () =
    let expenses = [ Expense.CarRental(1_000_000) ]
    let date = DateTime(2021, 10, 31, 22, 0, 0)

    let actual = ExpenseReport.create expenses date

    Assert.False((Seq.head actual.Entries).IsOverLimit)
