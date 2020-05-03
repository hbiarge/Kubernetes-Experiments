# Asp.Net Core and Kubernetes: Survival guide

This repo contains the materials for the talk *Asp.Net Core and Kubernetes: Survival guide* (spanish).

You can see the recorded video here: <https://youtu.be/07vU3exNKgY>

## Use with docker for windows

- Install docker for windows and enable the Kubernetes cluster.
- Install the NGinx ingress controller in the local cluster <https://kubernetes.github.io/ingress-nginx/deploy/.>
- Open a terminal and go to the `src` directory.
- Build the Mvc application image with this command: `docker build -f .\Mvc\Dockerfile -t web-demo:v1 .` This is the image referenced in all the samples.
- Go to the `demos` directory.
- Deploy the samples with the command `kubectl apply -f <sample-to-deploy>`
- Remove the samppes with the command `kubectl delete -f <sample-to-deploy>`
