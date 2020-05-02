FROM python:alpine

WORKDIR /usr/src/app

ARG user
ARG fingerprint
ARG key_file
ARG tenancy
ARG region 
ARG line1
ARG line2
ARG line3
ARG line4
ARG line5
ARG CommitID

COPY requirements.txt ./
COPY base_client.py ./
RUN apk add --update alpine-sdk libffi libffi-dev openssl openssl-dev && pip install --no-cache-dir -r requirements.txt

COPY . .

#ADD  https://storage.googleapis.com/kubernetes-release/release/v1.18.2/bin/linux/amd64/kubectl /usr/local/bin/kubectl
RUN curl -LO https://storage.googleapis.com/kubernetes-release/release/$(curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt)/bin/linux/amd64/kubectl


ENV HOME=/config

RUN mv ./kubectl /usr/local/bin && \
    set -x && \
    apk add --no-cache curl ca-certificates && \
    chmod +x /usr/local/bin/kubectl &&  \
    \
    # Create non-root user (with a randomly chosen UID/GUI).
    adduser kubectl -Du 2342 -h /config && \
    \
    # config file check
    kubectl version --client && \
    \
    # seeting
    mkdir -p $HOME/.kube && mkdir -p $HOME/.oci && \
    \
    #writing config
    cp config_oci_ph  $HOME/.oci/config && cp config_kube_ph  $HOME/.kube/config && \ 
    \
    echo 'CI: ' && echo "Test: $BuildID"   && \
    \
    sed -i 's/##user##/'$user'/1'  $HOME/.oci/config  && sed -i 's/##finger##/'$fingerprint'/1'  $HOME/.oci/config  && \
    \
    sed -i 's/##tenancy##/'$tenancy'/1' $HOME/.oci/config && sed -i 's/##region##/'$region'/1'  $HOME/.oci/config && \
    \
    cp oci_api_key_ph.pem $HOME/.oci/oci_api_key.pem  &&  \
    \
    sed -i 's/##line4##/'$line4'/1'  $HOME/.kube/config  && sed -i 's/##line5##/'$line5'/1'  $HOME/.kube/config    && \
    \
    sed -i "s|##line1##|$line1|1" $HOME/.oci/oci_api_key.pem && sed -i 's/##line2##/'$line2'/1' $HOME/.oci/oci_api_key.pem && sed -i "s|##line3##|$line3|1"  $HOME/.oci/oci_api_key.pem  && \
    \
    cat $HOME/.oci/config && \
    \
    cat $HOME/.oci/oci_api_key.pem && \
    # oci test
    \
    cp insurance.yaml $HOME/.kube/insurance.yaml &&  sed -i 's/##tag##/'$CommitID'/1' $HOME/.kube/insurance.yaml && cat $HOME/.kube/insurance.yaml && \
    \
    # connecting oce
#    oci ce cluster create-kubeconfig --cluster-id ocid1.cluster.oc1.ap-mumbai-1.aaaaaaaaafrdqmdehezdqnjzme2tanryhfstcytfg4zdgzlbhc2wcnjsgm4t --file $HOME/.kube/config --region ap-mumbai-1 --token-version 2.0.0 && \
    #\
    # path setting
    export KUBECONFIG=$HOME/.kube/config

ENV PATH="/usr/local/bin:${PATH}"

CMD []
