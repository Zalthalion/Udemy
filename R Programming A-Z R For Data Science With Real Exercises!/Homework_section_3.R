# Data
revenue <- c(14574.49, 7606.46, 8611.41, 9175.41, 8058.65, 8105.44, 11496.28, 9766.09, 10305.32, 14379.96, 10713.97, 15433.50)
expenses <- c(12051.82, 5695.07, 12319.20, 12089.72, 8658.57, 840.20, 3285.73, 5821.12, 6976.93, 16618.61, 10054.37, 3803.96)
months <- c("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December")
# Solution
# profit for each month
profit_before_tax <- revenue - expenses
# profit after tax for each month (the tax rate is 30%)
profit_after_tax <- round(profit_before_tax * 0.7, digits=2)
# profit margin for each month - equals to profit a after tax divided by revenue
profit_margin <- round(profit_after_tax / revenue,2) * 100
#  good months - where the profit after tax was greater than the mean for the year
good_months <- profit_after_tax > mean(profit_after_tax)
months[good_months]
# bad months - where the profit after tax was less than the mean for the year
bad_months <- !good_months
months[bad_months]
# the best month - where the profit after tax was max for the year
best_month <- profit_after_tax == max(profit_after_tax)
months[best_month]
# the worst month - where the profit after tax was min for the year
worst_month <- profit_after_tax == min(profit_after_tax)
months[worst_month]

# units of 1000
revenue_1000 <- round(revenue / 1000)
expenses_1000 <- round(expenses / 1000)
profit_before_tax_1000 <- round(profit_before_tax / 1000)
profit_after_tax_1000 <- round(profit_after_tax/1000)

# output
revenue_1000
expenses_1000
profit_before_tax_1000
profit_after_tax
months[good_months]
months[bad_months]
months[best_month]
months[worst_month]




