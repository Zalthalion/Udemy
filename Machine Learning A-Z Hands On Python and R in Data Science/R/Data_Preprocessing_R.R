#data preprocessing


#Importing the data from a data set
path = 'Data.csv'
dataset <- read.csv(path)

#Taking care of the missing data
#is.na()checks if the value is empty
#na.ra = TRUE asks to include empty values in the mean
dataset$Age = ifelse(is.na(dataset$Age), 
                     ave(dataset$Age, FUN = function(x) mean(x,na.rm = TRUE)),
                     dataset$Age)

dataset$Salary = ifelse(is.na(dataset$Salary), 
                     ave(dataset$Salary, FUN = function(x) mean(x,na.rm = TRUE)),
                     dataset$Salary)


#Encoding categorical data
#f1 gives description about methods

france = 'France'
spain = 'Spain'
germany = 'Germany'

yes = 'Yes'
no = 'No'

dataset$Country = factor(dataset$Country,
                         levels = c(france, spain, germany),
                         labels = c(1, 2, 3))

dataset$Purchased = factor(dataset$Purchased,
                         levels = c(yes, no),
                         labels = c(0, 1))

#Splitting the data set into test and training sets
#install.packages('caTools')
library(caTools)

set.seed(123)

split = sample.split(dataset$Purchased, SplitRatio = 0.8)

training_set = subset(dataset, split == TRUE)
test_set = subset(dataset, split == FALSE)

#feature scaling
training_set[,2:3] = scale(training_set[,2:3])
test_set[,2:3] = scale(test_set[,2:3])