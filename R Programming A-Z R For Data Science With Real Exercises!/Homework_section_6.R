data <- read.csv("datasets/Section6-Homework-Data.csv")
str(data)

data$Genre<-factor(data$Genre)
data$Studio<-factor(data$Studio)

# data filtering
genre_filter <- data$Genre %in% c("action", "adventure", "animation", "comedy", "drama")
studio_filter <- data$Studio %in% c("Buena Vista Studios", "WB", "Fox", "Universal", "Sony", "Paramount Pictures")

movies <- data[genre_filter & studio_filter,]

library(ggplot2)

p <- ggplot(data=movies, aes(x=Genre, y=Gross...US))

q <- p + geom_jitter(aes(colour=Studio)) + 
  geom_boxplot(alpha=0.7, outlier.colour = NA)

q <- q +
  xlab("Genre") +
  ylab("Gross % US") +
  ggtitle("Domestic Gross % by Genre")