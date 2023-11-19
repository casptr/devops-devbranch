pipeline {
    agent any

    stages {
        stage('Clone the repo') {
            steps {
                echo 'Clone the repository'
                sh 'rm -fr devops-devbranch'
                sh 'git clone https://github.com/casptr/devops-devbranch.git'
                sh 'ls'
            }
        stage('Preparation') {
            steps {
                build job: 'CheckIfDbRunning'
            }
        }
        stage('Deploy') {
            steps {
                build job: 'StartContainer'
            }
        }
    }
}
