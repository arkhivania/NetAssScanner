docker build -t nailblazor .
docker run -d -p 5003:5001 --name nailblazor nailblazor --restart always --memory="128m"
