# Must run Section4-Homework-Data.R and s4-BasketballData.R before this
myplot <- function(z, who=1:10)
{
  matplot(t(z[who,,drop=F]), type="b", pch=15:18, col=c(1:4,6), main="Basketball Player Analysis")
  legend("bottomleft", inset=0.01, legend=Players[who], col=c(1:4,6), pch=15:18, horiz=F)
}

myplot(FreeThrows)

# checking free throw attempts per game
myplot(FreeThrowAttempts/Games)

# free throw accuracy
myplot(FreeThrows/FreeThrowAttempts)

# player style patterns Excl. free throws
myplot((Points - FreeThrows)/FieldGoals)


#You have been supplied data for two more additional in-game statistics:
#  Free Throws
# Free Throw Attempts
#You need to create three plots that portray the following insights:
 #  Free Throw Attempts per game
# Accuracy of Free Throws
# Player playing style (2 vs 3 points preference) excluding Free Throws
 # Each Free Throw is worth 1 point
#The data has been supplied in the form of vectors. You will have to create the two
#matrices before you proceed with the analysis