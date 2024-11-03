# reapchickclient-asp

Create docker hub repository - publish
```
docker build -t reapchickclient-asp-api . 
docker run -it --rm -p 5184:80 --name reapchickclient-asp_container reapchickclient-asp-api
docker run -d --restart=always --name reapchickclient-asp_container -p 5184:80 reapchickclient-asp-api
docker run -d --restart=always -v d:/volumes/reapchickclient-asp/uploading:/app/uploading --name reapchickclient-asp_container -p 5184:80 reapchickclient-asp-api
docker run -d --restart=always -v /volumes/reapchickclient-asp/uploading:/app/uploading --name reapchickclient-asp_container -p 5184:80 reapchickclient-asp-api
docker ps -a
docker stop reapchickclient-asp_container
docker rm reapchickclient-asp_container

docker images --all
docker rmi reapchickclient-asp-api

docker login
docker tag reapchickclient-asp-api:latest reapchik/reapchickclient-asp-api:latest
docker push reapchik/reapchickclient-asp-api:latest

docker pull reapchik/reapchickclient-asp-api:latest
docker ps -a
docker run -d --restart=always --name reapchickclient-asp_container -p 5184:80 reapchik/reapchickclient-asp-api

docker run -d --restart=always -v /volumes/reapchickclient-asp/uploading:/app/uploading --name reapchickclient-asp_container -p 5184:80 reapchik/reapchickclient-asp-api


docker pull reapchik/reapchickclient-asp-api:latest
docker images --all
docker ps -a
docker stop reapchickclient-asp_container
docker rm reapchickclient-asp_container
docker run -d --restart=always --name reapchickclient-asp_container -p 5184:80 reapchik/reapchickclient-asp-api
```

```nginx options /etc/nginx/sites-available/default
server {
    server_name   api-reapchickclient-asp.itstep.click *.api-reapchickclient-asp.itstep.click;
    location / {
       proxy_pass         http://localhost:5184;
       proxy_http_version 1.1;
       proxy_set_header   Upgrade $http_upgrade;
       proxy_set_header   Connection keep-alive;
       proxy_set_header   Host $host;
       proxy_cache_bypass $http_upgrade;
       proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
       proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

server {
		server_name   qubix.itstep.click *.qubix.itstep.click;
		root /var/dist;
		index index.html;

		location / {
			try_files $uri /index.html;
			#try_files $uri $uri/ =404;
		}
}

server {
		server_name   admin-qubix.itstep.click *.admin-qubix.itstep.click;
		root /var/admin-qubix.itstep.click;
		index index.html;

		location / {
			try_files $uri /index.html;
			#try_files $uri $uri/ =404;
		}
}

sudo systemctl restart nginx
certbot
```

/var/api-qubix.itstep.click/



C:\Users\User\Desktop\platform-tools\minicap-2.4.0\jni\minicap-shared\aosp\libs\android-28\x86_64\minicap.so