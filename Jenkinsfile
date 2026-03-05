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
                    screen -r auth -X quit 2>/dev/null || true
                    screen -r menu -X quit 2>/dev/null || true
                    screen -r order -X quit 2>/dev/null || true
                    sleep 3
                '''
            }
        }

        stage('Deploy Auth Service') {
            steps {
                echo '🚀 Starting Auth Service...'
                sh '''
                    screen -dmS auth bash -c "
                        cd $WORKSPACE/AuthService.API
                        set -a && source .env && set +a
                        dotnet run --configuration Release
                    "
                    sleep 5
                '''
            }
        }

        stage('Deploy Menu Service') {
            steps {
                echo '🚀 Starting Menu Service...'
                sh '''
                    screen -dmS menu bash -c "
                        cd $WORKSPACE/MenuService.API
                        set -a && source .env && set +a
                        dotnet run --configuration Release
                    "
                    sleep 5
                '''
            }
        }

        stage('Deploy Order Service') {
            steps {
                echo '🚀 Starting Order Service...'
                sh '''
                    screen -dmS order bash -c "
                        cd $WORKSPACE/OrderService.API
                        set -a && source .env && set +a
                        dotnet run --configuration Release
                    "
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
            echo '❌ Pipeline failed! Check logs above.'
        }
    }
}
