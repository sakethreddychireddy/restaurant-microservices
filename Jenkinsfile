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
            AUTH_PID=$!
            echo $AUTH_PID > auth.pid
            echo "Auth Service started with PID: $AUTH_PID"
            sleep 8
            if kill -0 $AUTH_PID 2>/dev/null; then
                echo "✅ Auth Service is running"
            else
                echo "❌ Auth Service failed to start"
                cat auth.log
                exit 1
            fi
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
            MENU_PID=$!
            echo $MENU_PID > menu.pid
            echo "Menu Service started with PID: $MENU_PID"
            sleep 8
            if kill -0 $MENU_PID 2>/dev/null; then
                echo "✅ Menu Service is running"
            else
                echo "❌ Menu Service failed to start"
                cat menu.log
                exit 1
            fi
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
            ORDER_PID=$!
            echo $ORDER_PID > order.pid
            echo "Order Service started with PID: $ORDER_PID"
            sleep 8
            if kill -0 $ORDER_PID 2>/dev/null; then
                echo "✅ Order Service is running"
            else
                echo "❌ Order Service failed to start"
                cat order.log
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
            echo '❌ Pipeline failed!'
        }
    }
}


