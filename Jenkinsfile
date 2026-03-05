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
            sudo systemctl stop auth-service || true
            sudo systemctl stop menu-service || true
            sudo systemctl stop order-service || true
            sleep 3
        '''
    }
}

stage('Deploy Auth Service') {
    steps {
        echo '🚀 Starting Auth Service...'
        sh '''
            sudo systemctl start auth-service
            sleep 5
            sudo systemctl is-active auth-service && echo "✅ Auth Service is running" || echo "❌ Auth Service failed"
        '''
    }
}

stage('Deploy Menu Service') {
    steps {
        echo '🚀 Starting Menu Service...'
        sh '''
            sudo systemctl start menu-service
            sleep 5
            sudo systemctl is-active menu-service && echo "✅ Menu Service is running" || echo "❌ Menu Service failed"
        '''
    }
}

stage('Deploy Order Service') {
    steps {
        echo '🚀 Starting Order Service...'
        sh '''
            sudo systemctl start order-service
            sleep 5
            sudo systemctl is-active order-service && echo "✅ Order Service is running" || echo "❌ Order Service failed"
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

