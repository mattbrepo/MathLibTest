m1 <-read.csv("m1.csv", header=FALSE)
m2 <-read.csv("m2.csv", header=FALSE)
m1 = as.matrix(m1)
m2 = as.matrix(m2)

mRes = m1 %*% m2
write.table(mRes, "mResFromR.csv", sep=',', row.names=FALSE, col.names=FALSE)


test1<-replicate(1024, rnorm(1024))