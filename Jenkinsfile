pipeline {
    agent any
    environment {
        IMAGE_NAME = "identity_be"
        CONTAINER_NAME = "identity-be"
        PORT_HOST = "1000"
        PORT_CONTAINER = "8081"
        REPO_URL = "https://github.com/LHTrungSkySP/lht.Identity-OAuth2-be.git"
    }
    stages {
        stage('Checkout branch') {
            steps {
                checkout([$class: 'GitSCM',
                    branches: [[name: env.BRANCH_NAME]],
                    userRemoteConfigs: [[
                        url: "${REPO_URL}",
                        credentialsId: env.GIT_CRED_ID
                    ]]
                ])
            }
            post {
                failure {
                    echo "Checkout project FAILURE"
                }
            }
        }
        stage('Build Image') {
            steps {
                sh "docker build -t ${IMAGE_NAME}:latest ."
            }
        }
        stage('Deploy Container') {
            steps {
                sh '''
                    docker rm -f ${CONTAINER_NAME} || true
                    docker run -d --name ${CONTAINER_NAME} \
                    --network postgresql_internal_net -p ${PORT_HOST}:${PORT_CONTAINER} ${IMAGE_NAME}:latest
                '''
            }
        }
    }
}