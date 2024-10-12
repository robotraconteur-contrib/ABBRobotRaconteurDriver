FROM ubuntu:jammy AS build-env
WORKDIR /app
RUN apt update && apt install wget sudo software-properties-common -y
RUN apt install dotnet-sdk-6.0 -y
COPY . ./
RUN dotnet publish /p:Configuration=Release /p:Platform="Any CPU" -o output

FROM ubuntu:jammy
WORKDIR /app
COPY --from=build-env /app/output /opt/abb_robotraconteur_driver
COPY ./config/*.yml /config/

ENV DEBIAN_FRONTEND=noninteractive
ENV ROBOT_INFO_FILE=/config/abb_1200_5_90_robot_default_config.yml

RUN apt-get update && apt-get install wget sudo software-properties-common -y

RUN sudo apt install dotnet-runtime-6.0 -y

RUN sudo add-apt-repository ppa:robotraconteur/ppa -y \
    && sudo apt-get update \
    && sudo apt-get install librobotraconteur-net-native -y

CMD exec /opt/abb_robotraconteur_driver/ABBRobotRaconteurDriver --robot-info-file=$ROBOT_INFO_FILE
