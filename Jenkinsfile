pipeline {
    agent any
    environment {
        IMAGE_NAME = "testdocker_multi"
        CONTAINER_NAME = "testdocker_multi"
        PORT_HOST = "2000"
        PORT_CONTAINER = "8080"
        REPO_URL = "https://github.com/LHTrungSkySP/TestDocker.git"
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
                    docker run -d --name ${CONTAINER_NAME} -p ${PORT_HOST}:${PORT_CONTAINER} ${IMAGE_NAME}:latest
                '''
            }
        }
    }
}