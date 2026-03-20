pipeline {
    agent any

    environment {
        GOOGLE_CLIENT_ID            = credentials('AUTH_GOOGLE_CLIENT_ID')
        GOOGLE_CLIENT_SECRET        = credentials('AUTH_GOOGLE_CLIENT_SECRET')
        GITHUB_CLIENT_ID            = credentials('AUTH_GITHUB_CLIENT_ID')
        GITHUB_CLIENT_SECRET        = credentials('AUTH_GITHUB_CLIENT_SECRET')
        FRONTEND_BASE_URL           = credentials('FRONTEND_BASE_URL')
        NOTIFICATION_EMAIL          = credentials('NOTIFICATION_EMAIL')
        NOTIFICATION_EMAIL_PASSWORD = credentials('NOTIFICATION_EMAIL_PASSWORD')
        NOTIFICATION_FROM_NAME      = credentials('NOTIFICATION_FROM_NAME')
    }

    stages {

        stage('Checkout') {
            steps {
                echo 'Checking out latest code...'
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                echo 'Restoring NuGet dependencies...'
                sh 'dotnet restore Restaurant.sln'
            }
        }

        stage('Build') {
            steps {
                echo 'Building solution in Release mode...'
                sh 'dotnet build Restaurant.sln --no-restore --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing all services...'
                sh '''
                    dotnet publish AuthService.API/AuthService.API.csproj \
                        -c Release -o /home/saketh/publish/auth

                    dotnet publish MenuService.API/MenuService.API.csproj \
                        -c Release -o /home/saketh/publish/menu

                    dotnet publish OrderService.API/OrderService.API.csproj \
                        -c Release -o /home/saketh/publish/order

                    dotnet publish NotificationService.API/NotificationService.API.csproj \
                        -c Release -o /home/saketh/publish/notification
                '''
            }
        }

        stage('Inject Credentials') {
            steps {
                echo 'Injecting environment credentials...'
                sh """
                    cat > /home/saketh/publish/auth/.env << 'ENVEOF'
ASPNETCORE_URLS=http://0.0.0.0:5270
OAuth__Google__ClientId=${GOOGLE_CLIENT_ID}
OAuth__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
OAuth__GitHub__ClientId=${GITHUB_CLIENT_ID}
OAuth__GitHub__ClientSecret=${GITHUB_CLIENT_SECRET}
Frontend__BaseUrl=${FRONTEND_BASE_URL}
AllowedOrigins__0=http://192.168.1.213
AllowedOrigins__1=http://192.168.1.213:5270
Email__FromEmail=${NOTIFICATION_EMAIL}
Email__AppPassword=${NOTIFICATION_EMAIL_PASSWORD}
Email__FromName=${NOTIFICATION_FROM_NAME}
Email__Host=smtp.gmail.com
Email__Port=587
ENVEOF
                    echo "Auth Service credentials injected"

                    cat > /home/saketh/publish/menu/.env << 'ENVEOF'
ASPNETCORE_URLS=http://0.0.0.0:5271
AllowedOrigins__0=http://192.168.1.213
AllowedOrigins__1=http://192.168.1.213:5271
ENVEOF
                    echo "Menu Service credentials injected"

                    cat > /home/saketh/publish/order/.env << 'ENVEOF'
ASPNETCORE_URLS=http://0.0.0.0:5272
AllowedOrigins__0=http://192.168.1.213
AllowedOrigins__1=http://192.168.1.213:5272
ENVEOF
                    echo "Order Service credentials injected"

                    cat > /home/saketh/publish/notification/.env << 'ENVEOF'
ASPNETCORE_URLS=http://0.0.0.0:5273
Email__FromEmail=${NOTIFICATION_EMAIL}
Email__AppPassword=${NOTIFICATION_EMAIL_PASSWORD}
Email__FromName=${NOTIFICATION_FROM_NAME}
Email__Host=smtp.gmail.com
Email__Port=587
AllowedOrigins__0=http://192.168.1.213
ENVEOF
                    echo "Notification Service credentials injected"
                """
            }
        }

        stage('Stop Services') {
            steps {
                echo 'Stopping running services gracefully...'
                sh '''
                    sudo systemctl stop auth-service         || true
                    sudo systemctl stop menu-service         || true
                    sudo systemctl stop order-service        || true
                    sudo systemctl stop notification-service || true
                    sleep 3
                    echo "All services stopped"
                '''
            }
        }

        stage('Deploy') {
            steps {
                echo 'Starting all services...'
                sh '''
                    sudo systemctl start auth-service
                    sleep 5
                    if sudo systemctl is-active --quiet auth-service; then
                        echo "Auth Service is running"
                    else
                        echo "Auth Service failed to start"
                        sudo journalctl -u auth-service -n 20
                        exit 1
                    fi

                    sudo systemctl start menu-service
                    sleep 5
                    if sudo systemctl is-active --quiet menu-service; then
                        echo "Menu Service is running"
                    else
                        echo "Menu Service failed to start"
                        sudo journalctl -u menu-service -n 20
                        exit 1
                    fi

                    sudo systemctl start order-service
                    sleep 5
                    if sudo systemctl is-active --quiet order-service; then
                        echo "Order Service is running"
                    else
                        echo "Order Service failed to start"
                        sudo journalctl -u order-service -n 20
                        exit 1
                    fi

                    sudo systemctl start notification-service
                    sleep 5
                    if sudo systemctl is-active --quiet notification-service; then
                        echo "Notification Service is running"
                    else
                        echo "Notification Service failed to start"
                        sudo journalctl -u notification-service -n 20
                        exit 1
                    fi
                '''
            }
        }

        stage('Health Check') {
            steps {
                echo 'Running health checks...'
                sh '''
                    sleep 3

                    AUTH=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5270/api/auth/health)
                    if [ "$AUTH" = "200" ]; then
                        echo "Auth Service healthy"
                    else
                        echo "Auth Service health check returned $AUTH"
                    fi

                    MENU=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5271/api/menuitems/available)
                    if [ "$MENU" = "200" ]; then
                        echo "Menu Service healthy"
                    else
                        echo "Menu Service health check returned $MENU"
                    fi

                    ORDER=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5272/api/orders/health 2>/dev/null || echo "000")
                    echo "Order Service responded with $ORDER"

                    NOTIFICATION=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5273/api/health)
                    if [ "$NOTIFICATION" = "200" ]; then
                        echo "Notification Service healthy"
                    else
                        echo "Notification Service health check returned $NOTIFICATION"
                    fi
                '''
            }
        }

    }

    post {
        success {
            echo 'Deployment successful! All services are live.'
        }
        failure {
            echo 'Deployment failed! Check logs above.'
        }
    }
}