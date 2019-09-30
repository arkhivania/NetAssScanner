docker build -t nailblazor .
docker run -d -p 5003:5001 --restart always --name nailblazor nailblazor
