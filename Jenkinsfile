pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                echo '📦 Checking out code...'
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                echo '🔄 Restoring dependencies...'
                sh 'dotnet restore Restaurant.sln'
            }
        }

        stage('Build') {
            steps {
                echo '🔨 Building solution...'
                sh 'dotnet build Restaurant.sln --no-restore --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing services...'
                sh '''
                    dotnet publish AuthService.API/AuthService.API.csproj -c Release -o /home/saketh/publish/auth
                    dotnet publish MenuService.API/MenuService.API.csproj -c Release -o /home/saketh/publish/menu
                    dotnet publish OrderService.API/OrderService.API.csproj -c Release -o /home/saketh/publish/order
                '''
            }
        }

        stage('Stop Services') {
            steps {
                echo '🛑 Stopping existing services...'
                sh '''
                    [ -f /home/saketh/pids/auth.pid ] && kill $(cat /home/saketh/pids/auth.pid) 2>/dev/null || true
                    [ -f /home/saketh/pids/menu.pid ] && kill $(cat /home/saketh/pids/menu.pid) 2>/dev/null || true
                    [ -f /home/saketh/pids/order.pid ] && kill $(cat /home/saketh/pids/order.pid) 2>/dev/null || true
                    sleep 3
                '''
            }
        }

        stage('Deploy Auth Service') {
            steps {
                echo '🚀 Starting Auth Service...'
                sh '''#!/bin/bash
                    mkdir -p /home/saketh/pids
                    mkdir -p /home/saketh/logs
                    cp /home/saketh/.jenkins/workspace/restaurant/AuthService.API/.env /home/saketh/publish/auth/.env
                    cd /home/saketh/publish/auth
                    set -a && . .env && set +a
                    nohup dotnet AuthService.API.dll > /home/saketh/logs/auth.log 2>&1 &
                    echo $! > /home/saketh/pids/auth.pid
                    sleep 8
                    if kill -0 $(cat /home/saketh/pids/auth.pid) 2>/dev/null; then
                        echo "✅ Auth Service is running"
                    else
                        echo "❌ Auth Service failed"
                        cat /home/saketh/logs/auth.log
                        exit 1
                    fi
                '''
            }
        }

        stage('Deploy Menu Service') {
            steps {
                echo '🚀 Starting Menu Service...'
                sh '''#!/bin/bash
                    mkdir -p /home/saketh/pids
                    mkdir -p /home/saketh/logs
                    cp /home/saketh/.jenkins/workspace/restaurant/MenuService.API/.env /home/saketh/publish/menu/.env
                    cd /home/saketh/publish/menu
                    set -a && . .env && set +a
                    nohup dotnet MenuService.API.dll > /home/saketh/logs/menu.log 2>&1 &
                    echo $! > /home/saketh/pids/menu.pid
                    sleep 8
                    if kill -0 $(cat /home/saketh/pids/menu.pid) 2>/dev/null; then
                        echo "✅ Menu Service is running"
                    else
                        echo "❌ Menu Service failed"
                        cat /home/saketh/logs/menu.log
                        exit 1
                    fi
                '''
            }
        }

        stage('Deploy Order Service') {
            steps {
                echo '🚀 Starting Order Service...'
                sh '''#!/bin/bash
                    mkdir -p /home/saketh/pids
                    mkdir -p /home/saketh/logs
                    cp /home/saketh/.jenkins/workspace/restaurant/OrderService.API/.env /home/saketh/publish/order/.env
                    cd /home/saketh/publish/order
                    set -a && . .env && set +a
                    nohup dotnet OrderService.API.dll > /home/saketh/logs/order.log 2>&1 &
                    echo $! > /home/saketh/pids/order.pid
                    sleep 8
                    if kill -0 $(cat /home/saketh/pids/order.pid) 2>/dev/null; then
                        echo "✅ Order Service is running"
                    else
                        echo "❌ Order Service failed"
                        cat /home/saketh/logs/order.log
                        exit 1
                    fi
                '''
            }
        }

        stage('Done') {
            steps {
                echo '✅ Deployment complete!'
            }
        }
    }

    post {
        success {
            echo '✅ Pipeline succeeded!'
        }
        failure {
            echo '❌ Pipeline failed! Check logs above.'
        }
    }
}
