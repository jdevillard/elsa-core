### Jaeger for Telemetry

```
docker run -d --name jaeger \
  -p 5775:5775/udp \
  -p 6831:6831/udp \
  -p 6832:6832/udp \
  -p 5778:5778 \
  -p 16686:16686 \
  -p 14268:14268 \
  -p 14250:14250 \
  -p 9411:9411 \
  jaegertracing/all-in-one:latest
```

### testing

if running from docker (windows): 

`cat "{PathToFile}k6test.js" | docker run --rm -i --add-host=host.docker.internal:host-gateway grafana/k6 run --vus 5 --duration 10s - `

if running from docker 

`$ docker run --rm -i grafana/k6 run --vus 5 --duration 10s - <k6test.js`

if running from package or binary
	
` k6 run --vus 5 --duration 10s k6test.js`

### script file :

_Change the host of the url to match your need - this is added when running test from docker and elsa from local machine in debug_

import http from 'k6/http';

export default function () {
    http.get('http://host.docker.internal:58522/workflows/hello-world');
}