pipeline {
    agent any

    environment {
        GOOGLE_CLIENT_ID     = credentials('AUTH_GOOGLE_CLIENT_ID')
        GOOGLE_CLIENT_SECRET = credentials('AUTH_GOOGLE_CLIENT_SECRET')
        GITHUB_CLIENT_ID     = credentials('AUTH_GITHUB_CLIENT_ID')
        GITHUB_CLIENT_SECRET = credentials('AUTH_GITHUB_CLIENT_SECRET')
        FRONTEND_BASE_URL    = credentials('FRONTEND_BASE_URL')
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
                '''
            }
        }

        stage('Inject Credentials') {
            steps {
                echo 'Injecting environment credentials...'
                sh '''
                    # ── Auth Service env ──────────────────────────────────
                    cat > /home/saketh/publish/auth/.env << EOF
ASPNETCORE_URLS=http://0.0.0.0:5270
OAuth__Google__ClientId=${GOOGLE_CLIENT_ID}
OAuth__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
OAuth__GitHub__ClientId=${GITHUB_CLIENT_ID}
OAuth__GitHub__ClientSecret=${GITHUB_CLIENT_SECRET}
Frontend__BaseUrl=${FRONTEND_BASE_URL}
EOF
                    echo "Auth Service credentials injected"

                    # ── Menu Service env ──────────────────────────────────
                    cat > /home/saketh/publish/menu/.env << EOF
ASPNETCORE_URLS=http://0.0.0.0:5271
EOF
                    echo "Menu Service credentials injected"

                    # ── Order Service env ─────────────────────────────────
                    cat > /home/saketh/publish/order/.env << EOF
ASPNETCORE_URLS=http://0.0.0.0:5272
EOF
                    echo "Order Service credentials injected"
                '''
            }
        }

        stage('Stop Services') {
            steps {
                echo 'Stopping running services gracefully...'
                sh '''
                    sudo systemctl stop auth-service  || true
                    sudo systemctl stop menu-service  || true
                    sudo systemctl stop order-service || true
                    sleep 3
                    echo "All services stopped"
                '''
            }
        }

        stage('Deploy') {
            steps {
                echo 'Starting all services...'
                sh '''
                    # ── Auth Service ──────────────────────────────────────
                    sudo systemctl start auth-service
                    sleep 5
                    if sudo systemctl is-active --quiet auth-service; then
                        echo "Auth Service is running"
                    else
                        echo "Auth Service failed to start"
                        sudo journalctl -u auth-service -n 20
                        exit 1
                    fi

                    # ── Menu Service ──────────────────────────────────────
                    sudo systemctl start menu-service
                    sleep 5
                    if sudo systemctl is-active --quiet menu-service; then
                        echo "Menu Service is running"
                    else
                        echo "Menu Service failed to start"
                        sudo journalctl -u menu-service -n 20
                        exit 1
                    fi

                    # ── Order Service ─────────────────────────────────────
                    sudo systemctl start order-service
                    sleep 5
                    if sudo systemctl is-active --quiet order-service; then
                        echo "Order Service is running"
                    else
                        echo "Order Service failed to start"
                        sudo journalctl -u order-service -n 20
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

                    # ── Auth Service ──────────────────────────────────────
                    AUTH=$(curl -s -o /dev/null -w "%{http_code}" \
                        http://localhost:5270/api/auth/health)
                    if [ "$AUTH" = "200" ]; then
                        echo "Auth Service healthy"
                    else
                        echo "Auth Service health check returned $AUTH"
                    fi

                    # ── Menu Service ──────────────────────────────────────
                    MENU=$(curl -s -o /dev/null -w "%{http_code}" \
                        http://localhost:5271/api/menuitems/available)
                    if [ "$MENU" = "200" ]; then
                        echo "Menu Service healthy"
                    else
                        echo "Menu Service health check returned $MENU"
                    fi

                    # ── Order Service ─────────────────────────────────────
                    ORDER=$(curl -s -o /dev/null -w "%{http_code}" \
                        http://localhost:5272/api/orders/health 2>/dev/null || echo "000")
                    echo "Order Service responded with $ORDER"
                '''
            }
        }

    }

    post {
        success {
            echo '''
               Deployment Successful!
               All services are live
            '''
        }
        failure {
            echo '''
               Deployment Failed!
               Check logs above for details
            '''
        }
    }
}
