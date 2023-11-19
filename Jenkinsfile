pipeline {
    agent any

    stages {
        stage('Preparation') {
            steps {
                build job: 'CheckIfDbRunning'
                build job: 'BuildApp'
            }
        }
        stage('Deploy') {
            steps {
                build job: 'StartContainer'
            }
        }
    }
}
