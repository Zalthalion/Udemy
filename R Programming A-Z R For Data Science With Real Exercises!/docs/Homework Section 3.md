# Homework Section 3

## **Scenario:**
You are a Data Scientist working for a consulting firm. One of your
colleagues from the Auditing department has asked you to help them assess the
financial statement of organisation X.
You have been supplied with two vectors of data: monthly revenue and monthly
expenses for the financial year in question. Your task is to calculate the following
financial metrics:
- profit for each month
- profit after tax for each month (the tax rate is 30%)
- profit margin for each month - equals to profit a after tax divided by revenue
- good months - where the profit after tax was greater than the mean for the year
- bad months - where the profit after tax was less than the mean for the year
- the best month - where the profit after tax was max for the year
- the worst month - where the profit after tax was min for the year

All results need to be presented as vectors.
Results for dollar values need to be calculated with $0.01 precision, but need to be
presented in Units of $1,000 (i.e. 1k) with no decimal points.
Results for the profit margin ratio need to be presented in units of % with no
decimal points.
Note: You colleague has warned you that it is okay for tax for any given month to be
negative (in accounting terms, negative tax translates into a deferred tax asset).

## **My results:**
```R
> # output
> revenue_1000
 [1] 15  8  9  9  8  8 11 10 10 14 11 15
> expenses_1000
 [1] 12  6 12 12  9  1  3  6  7 17 10  4
> profit_before_tax_1000
 [1]  3  2 -4 -3 -1  7  8  4  3 -2  1 12
> profit_after_tax
 [1]  1765.87  1337.97 -2595.45 -2040.02  -419.94  5085.67  5747.38  2761.48  2329.87 -1567.06   461.72  8140.68
> months[good_months]
[1] "January" "June" "July" "August" "September" "December" 
> months[bad_months]
[1] "February" "March" "April" "May" "October" "November"
> months[best_month]
[1] "December"
> months[worst_month]
[1] "March"
```