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

        stage('Stop Services') {
            steps {
                echo '🛑 Stopping existing services...'
                sh '''
                    [ -f $WORKSPACE/AuthService.API/auth.pid ] && kill $(cat $WORKSPACE/AuthService.API/auth.pid) 2>/dev/null || true
                    [ -f $WORKSPACE/MenuService.API/menu.pid ] && kill $(cat $WORKSPACE/MenuService.API/menu.pid) 2>/dev/null || true
                    [ -f $WORKSPACE/OrderService.API/order.pid ] && kill $(cat $WORKSPACE/OrderService.API/order.pid) 2>/dev/null || true
                    sleep 3
                '''
            }
        }

        stage('Deploy Auth Service') {
            steps {
                echo '🚀 Starting Auth Service...'
                sh '''
                    cd $WORKSPACE/AuthService.API
                    set -a && source .env && set +a
                    nohup dotnet run --configuration Release > auth.log 2>&1 &
                    echo $! > auth.pid
                    sleep 5
                '''
            }
        }

        stage('Deploy Menu Service') {
            steps {
                echo '🚀 Starting Menu Service...'
                sh '''
                    cd $WORKSPACE/MenuService.API
                    set -a && source .env && set +a
                    nohup dotnet run --configuration Release > menu.log 2>&1 &
                    echo $! > menu.pid
                    sleep 5
                '''
            }
        }

        stage('Deploy Order Service') {
            steps {
                echo '🚀 Starting Order Service...'
                sh '''
                    cd $WORKSPACE/OrderService.API
                    set -a && source .env && set +a
                    nohup dotnet run --configuration Release > order.log 2>&1 &
                    echo $! > order.pid
                    sleep 5
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
            echo '❌ Pipeline failed!'
        }
    }
}
