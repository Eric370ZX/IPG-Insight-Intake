
import hudson.tasks.test.AbstractTestResultAction
  
node {
	try {
		stage('Checkout')
	    	{
	        	git credentialsId: '0e602ad6-7681-425f-b8d8-179c634234e5', url: 'https://github.com/IPGDevelopment/IPG-Insight-Intake.git'
	    	}
		stage ('Build')
		{
			  bat "\"${tool 'Nuget'}\" restore Insight.Intake.sln"
				bat "\"${tool 'MsBuild.exe v2017'}\" Insight.Intake.sln /p:ProductVersion=1.0.0.${env.BUILD_NUMBER}"
				bat "cd ./Insight.Intake/WebResources/ && npm ci && npm run build-all"
		}
		stage ('UnitTests')
		{
			bat returnStatus: true, script: "\"C:/Program Files/dotnet/dotnet.exe\" test \"${workspace}/Insight.Intake.sln\" --logger \"trx;LogFileName=unit_tests.xml\" --no-build"
			step([$class: 'MSTestPublisher', testResultsFile:"**/unit_tests.xml", failOnError: true, keepLongStdio: true])
			def allTestPassed = allTestPass()
			if(allTestPassed == false)
			{
				slackNotifier('UNITTESTS')
				assert false
			}
			else
			{
				slackNotifier('SUCCESS')
			}
		}
	} 
  catch (Exception err) 
	{
    stage('Finishing') 
    {
      println "Pipeline Error: " + err      
      slackNotifier('FAILURE')
    	assert false
    }
	}
}

def allTestPass() 
{
  def testStatus = ""
	def allTestPass = 0
    	AbstractTestResultAction testResultAction = currentBuild.rawBuild.getAction(AbstractTestResultAction.class)
    
	if (testResultAction != null) 
	{
		def total = testResultAction.totalCount
		def failed = testResultAction.failCount
		def skipped = testResultAction.skipCount
		def passed = total - failed - skipped
    testStatus = "Test Status:\n  Passed: ${passed}, Failed: ${failed} ${testResultAction.failureDiffString}, Skipped: ${skipped}"

		if (failed == 0) 
		{
		    currentBuild.result = 'SUCCESS'
		}

		allTestPass = (failed == 0)
  }
	println testStatus
  return allTestPass
}

def slackNotifier(String buildResult) {
  println "Notify Slack With buildResult = " + buildResult
	if ( buildResult == "SUCCESS" ) {
		slackSend color: "good", message: "SUCCESSFUL: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"
	}
	else if( buildResult == "FAILURE" ) { 
		slackSend color: "danger", message: "FAILED: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"
	}
  	else if( buildResult == "UNITTESTS" ) { 
		slackSend color: "danger", message: "UNITTESTS FAILED: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"
	}
	else if( buildResult == "UNSTABLE" ) { 
		slackSend color: "warning", message: "UNSTABLE: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"
	}
	else {
		slackSend color: "danger", message: "UNCLEAR: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})"	
	}
}
