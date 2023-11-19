pipeline {
    agent any

    stages {
        stage('Clone the repository') {
            steps {
                echo 'Cloning repository'
                sh 'rm -fr devops-devbranch'
                sh 'git clone https://github.com/casptr/devops-devbranch.git'
                sh 'pwd'
            }
        }
        stage('Preparation') {
            steps {
                sh 'pwd'
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
