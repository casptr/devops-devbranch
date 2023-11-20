pipeline {
    agent any

    stages {
        stage('Preparation') {
            steps {
                echo 'Checking database container'
                build job: 'CheckIfDbRunning'
            }
        }
        stage ('Compile & Testing') {
            steps {
                echo 'Compiling app and running tests'
                build job: 'BuildApp'
            }
        }
        stage('Deployment') {
            steps {
                echo 'Deploying application container'
            }
        }
    }
}
