# Preparing dataset
movies <- read.csv("Movie Ratings.csv")
head(movies)
colnames(movies) <- c("Film", "Genre", "CriticRating", "AudienceRating", "BudgetMillions", "Year")
str(movies)
is.factor(movies$Genre)
movies$Genre <- factor(movies$Genre)
movies$Film <- factor(movies$Film)
movies$Year <- factor(movies$Year)

library(ggplot2)

dots <-ggplot(data=movies, aes(x=CriticRating, y=AudienceRating, colour=Genre, size=BudgetMillions)) +
  geom_point()

histo <- ggplot(data=movies, aes(x=BudgetMillions)) + 
  geom_histogram(binwidth = 10, aes(fill=Genre), colour = "Black") +
  xlab("Money Axis") +
  ylab("Number of Movies") +
  theme(axis.title.x = element_text(colour="DarkGreen", size = 20),
        axis.title.y = element_text(colour="Red", size=20),
        axis.text = element_text(size=10),
        legend.position = c(0.97,0.97),
        legend.justification = c(1,1))

density <- ggplot(data=movies, aes(x=BudgetMillions)) + 
  geom_density(aes(fill=Genre), position = "Stack")

audience_box <- ggplot(data = movies, aes(x=Genre, y=AudienceRating, colour=Genre)) +
  geom_jitter() +
  geom_boxplot(size=1.2, alpha=0.5)

critics_box <- ggplot(data = movies, aes(x=Genre, y=CriticRating, colour=Genre)) +
  geom_jitter() +
  geom_boxplot(size=1.2, alpha=0.5)

facets_histo <- ggplot(data=movies, aes(x=BudgetMillions, fill=Genre)) +
  geom_histogram(binwidth=10, colour="Black") +
  facet_grid(Genre~., scales="free")

facets_scatter <- ggplot(data=movies, aes(x=CriticRating, y=AudienceRating, colour=Genre)) +
  geom_point() +
  geom_smooth(fill=NA) +
  facet_grid(Genre~Year) +
  coord_cartesian(ylim=c(0,100))

