pipeline {
    agent any

    stages {
        stage('Preparation') {
            steps {
                echo 'Checking database container'
                build job: 'CheckIfDbRunning'
                echo 'Creating application container image'
                build job: 'BuildApp'
            }
        }
        stage('Deployment') {
            steps {
                echo 'Deploying application container'
                build job: 'StartContainer'
            }
        }
    }
}
